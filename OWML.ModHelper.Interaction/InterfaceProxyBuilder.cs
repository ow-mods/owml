using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace OWML.ModHelper.Interaction
{
    internal class InterfaceProxyBuilder
    {

        private readonly Type TargetType;

        private readonly Type ProxyType;

        public InterfaceProxyBuilder(string name, ModuleBuilder moduleBuilder, Type interfaceType, Type targetType)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (targetType == null)
                throw new ArgumentNullException(nameof(targetType));

            var proxyBuilder = moduleBuilder.DefineType(name, TypeAttributes.Public | TypeAttributes.Class);
            proxyBuilder.AddInterfaceImplementation(interfaceType);

            var targetField = proxyBuilder.DefineField("__Target", targetType, FieldAttributes.Private);

            CreateConstructor(proxyBuilder, targetField, targetType);

            foreach (var proxyMethod in interfaceType.GetMethods())
            {
                var targetMethod = targetType.GetMethod(proxyMethod.Name, proxyMethod.GetParameters().Select(a => a.ParameterType).ToArray());
                if (targetMethod == null)
                    throw new InvalidOperationException($"The {interfaceType.FullName} interface defines method {proxyMethod.Name} which doesn't exist in the API.");

                this.ProxyMethod(proxyBuilder, targetMethod, targetField);
            }

            this.TargetType = targetType;
            this.ProxyType = proxyBuilder.CreateType();
        }

        public object CreateInstance(object targetInstance)
        {
            var constructor = this.ProxyType.GetConstructor(new[] { this.TargetType });
            if (constructor == null)
                throw new InvalidOperationException($"Couldn't find the constructor for generated proxy type '{this.ProxyType.Name}'."); // should never happen
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
            for (int i = 0; i < argTypes.Length; i++)
                il.Emit(OpCodes.Ldarg, i + 1);
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
