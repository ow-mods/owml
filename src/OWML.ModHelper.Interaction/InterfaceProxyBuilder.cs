using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

// Adapted from SMAPI code : https://github.com/Pathoschild/SMAPI/tree/5a92b0cd357776eebb88e001384f9ca1ccdb7d5c/src/SMAPI/Framework/Reflection

namespace OWML.ModHelper.Interaction
{
	internal class InterfaceProxyBuilder
	{
		private readonly Type _targetType;
		private readonly Type _proxyType;

		public InterfaceProxyBuilder(string name, ModuleBuilder moduleBuilder, Type interfaceType, Type targetType)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}
			if (targetType == null)
			{
				throw new ArgumentNullException(nameof(targetType));
			}

			var proxyBuilder = moduleBuilder.DefineType(name, TypeAttributes.Public | TypeAttributes.Class);
			proxyBuilder.AddInterfaceImplementation(interfaceType);

			var targetField = proxyBuilder.DefineField("__Target", targetType, FieldAttributes.Private);

			CreateConstructor(proxyBuilder, targetField, targetType);

			var allTargetMethods = targetType.GetMethods().ToList();
			foreach (var targetInterface in targetType.GetInterfaces())
			{
				foreach (var targetMethod in targetInterface.GetMethods())
				{
					if (!targetMethod.IsAbstract)
						allTargetMethods.Add(targetMethod);
				}
			}

			bool AreGenericTypesMatching(Type a, Type b)
			{
				if (a.GenericTypeArguments.Length != b.GenericTypeArguments.Length)
				{
					return false;
				}

				if (a.GetGenericTypeDefinition() != b.GetGenericTypeDefinition())
				{
					return false;
				}

				for (var i = 0; i < a.GenericTypeArguments.Length; i++)
				{
					if (!AreTypesMatching(a.GenericTypeArguments[i], b.GenericTypeArguments[i]))
					{
						return false;
					}
				}

				return true;
			}

			bool AreTypesMatching(Type a, Type b)
			{
				if (a == null || b == null)
				{
					return false;
				}

				if (a.IsGenericParameter != b.IsGenericParameter
				    || a.IsGenericType != b.IsGenericType)
				{
					return false;
				}

				if (a.IsEnum && b.IsEnum)
				{
					if (Enum.GetNames(a).Length != Enum.GetNames(b).Length
					    || a.Name != b.Name
						|| a.GetEnumUnderlyingType() != b.GetEnumUnderlyingType())
					{
						return false;
					}

					for (var i = 0; i < Enum.GetNames(a).Length; i++)
					{
						var nameA = Enum.GetNames(a)[i];
						var nameB = Enum.GetNames(b)[i];

						if (nameA != nameB)
						{
							return false;
						}

						var valueA = Convert.ChangeType(Enum.Parse(a, nameA), a.GetEnumUnderlyingType());
						var valueB = Convert.ChangeType(Enum.Parse(b, nameB), b.GetEnumUnderlyingType());

						if (!valueA.Equals(valueB))
						{
							return false;
						}
					}

					return true;
				}

				if (a.IsGenericParameter)
				{
					// most likely the type is T
					return a.GenericParameterPosition == b.GenericParameterPosition;
				}

				if (a.IsGenericType)
				{
					// the type is something like Action<...>
					return AreGenericTypesMatching(a, b);
				}

				// this is just a normal type
				return a.IsAssignableFrom(b);
			}

			foreach (var proxyMethod in interfaceType.GetMethods())
			{
				var proxyMethodParameters = proxyMethod.GetParameters();
				var proxyMethodGenericArguments = proxyMethod.GetGenericArguments();
				var targetMethod = allTargetMethods.Where(m =>
				{
					if (m.Name != proxyMethod.Name)
						return false;
					if (m.GetGenericArguments().Length != proxyMethodGenericArguments.Length)
						return false;
					if (!AreTypesMatching(m.ReturnType, proxyMethod.ReturnType))
						return false;

					var mParameters = m.GetParameters();
					if (m.GetParameters().Length != proxyMethodParameters.Length)
						return false;
					for (int i = 0; i < mParameters.Length; i++)
					{
						if (!AreTypesMatching(mParameters[i].ParameterType, proxyMethodParameters[i].ParameterType))
							return false;
					}
					return true;
				}).FirstOrDefault();
				if (targetMethod == null)
				{
					throw new InvalidOperationException($"The {interfaceType.FullName} interface defines method {proxyMethod.Name} which doesn't exist in the API.");
				}
				ProxyMethod(proxyBuilder, proxyMethod, targetMethod, targetField);
			}

			_targetType = targetType;
			_proxyType = proxyBuilder.CreateType();
		}

		public object CreateInstance(object targetInstance)
		{
			var constructor = _proxyType.GetConstructor(new[] { _targetType });
			if (constructor == null)
			{
				throw new InvalidOperationException($"Couldn't find the constructor for generated proxy type '{_proxyType.Name}'."); // should never happen
			}
			return constructor.Invoke(new[] { targetInstance });
		}

		private void ProxyMethod(TypeBuilder proxyBuilder, MethodInfo proxy, MethodInfo target, FieldBuilder instanceField)
		{
			// create method
			var methodBuilder = proxyBuilder.DefineMethod(proxy.Name, MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual);
			// set up generic arguments
			var proxyGenericArguments = proxy.GetGenericArguments();
			var genericArgNames = proxyGenericArguments.Select(a => a.Name).ToArray();
			var genericTypeParameterBuilders = proxyGenericArguments.Length == 0 ? null : methodBuilder.DefineGenericParameters(genericArgNames);
			for (int i = 0; i < proxyGenericArguments.Length; i++)
				genericTypeParameterBuilders[i].SetGenericParameterAttributes(proxyGenericArguments[i].GenericParameterAttributes);

			// set up return type
			methodBuilder.SetReturnType(proxy.ReturnType.IsGenericParameter ? genericTypeParameterBuilders[proxy.ReturnType.GenericParameterPosition] : proxy.ReturnType);

			// set up parameters
			var argTypes = proxy.GetParameters()
				.Select(a => a.ParameterType)
				.Select(t => t.IsGenericParameter ? genericTypeParameterBuilders[t.GenericParameterPosition] : t)
				.ToArray();
			methodBuilder.SetParameters(argTypes);

			CreateProxyMethodBody(methodBuilder, target, instanceField, argTypes);
		}

		private void CreateProxyMethodBody(MethodBuilder methodBuilder, MethodInfo target, FieldBuilder instanceField, Type[] argTypes)
		{
			var il = methodBuilder.GetILGenerator();

			// load target instance
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldfld, instanceField);

			// invoke target method on instance
			for (var i = 0; i < argTypes.Length; i++)
			{
				il.Emit(OpCodes.Ldarg, i + 1);
			}
			il.Emit(OpCodes.Call, target);

			// return result
			il.Emit(OpCodes.Ret);

			// Equivalent code :

			// return __Target.methodname(arg1, arg2, ...);
		}

		private void CreateConstructor(TypeBuilder proxyBuilder, FieldBuilder targetField, Type targetType)
		{
			var constructor = proxyBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard | CallingConventions.HasThis, new[] { targetType });
			var il = constructor.GetILGenerator();

			il.Emit(OpCodes.Ldarg_0); // this
			il.Emit(OpCodes.Call, typeof(object).GetConstructor(new Type[0])); // call base constructor
			il.Emit(OpCodes.Ldarg_0);      // this
			il.Emit(OpCodes.Ldarg_1);      // load argument
			il.Emit(OpCodes.Stfld, targetField); // set field to loaded argument
			il.Emit(OpCodes.Ret);

			// Equivalent code :

			/*
			 *	public ctor(TargetType targetType)
			 *	{
			 *		this.__Target = targetType;
			 *	}
			 */
		}
	}
}
