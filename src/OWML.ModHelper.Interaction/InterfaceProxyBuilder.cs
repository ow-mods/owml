using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

// Adapted from SMAPI code : https://github.com/Pathoschild/SMAPI/tree/0ff82c38e7a5b630256d2cd23a63ac1088d13e39/src/SMAPI/Framework/Reflection

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

			foreach (var proxyMethod in interfaceType.GetMethods())
			{
				var proxyMethodParameters = proxyMethod.GetParameters();
				var targetMethod = allTargetMethods.Where(m =>
				{
					if (m.Name != proxyMethod.Name)
						return false;
					if (m.ReturnType != proxyMethod.ReturnType)
						return false;

					var mParameters = m.GetParameters();
					if (m.GetParameters().Length != proxyMethodParameters.Length)
						return false;
					for (int i = 0; i < mParameters.Length; i++)
					{
						if (!mParameters[i].ParameterType.IsAssignableFrom(proxyMethodParameters[i].ParameterType))
							return false;
					}
					return true;
				}).FirstOrDefault();
				if (targetMethod == null)
				{
					throw new InvalidOperationException($"The {interfaceType.FullName} interface defines method {proxyMethod.Name} which doesn't exist in the API.");
				}
				ProxyMethod(proxyBuilder, targetMethod, targetField);
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

		private void ProxyMethod(TypeBuilder proxyBuilder, MethodInfo target, FieldBuilder instanceField)
		{
			var argTypes = target.GetParameters().Select(a => a.ParameterType).ToArray();

			var methodBuilder = proxyBuilder.DefineMethod(target.Name, MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual);
			methodBuilder.SetParameters(argTypes);
			methodBuilder.SetReturnType(target.ReturnType);

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
		}
	}
}
