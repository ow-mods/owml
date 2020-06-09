using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace OWML.ModHelper.Interaction
{
    public class InterfaceProxyFactory
    {
        private readonly ModuleBuilder ModuleBuilder;

        private readonly IDictionary<string, InterfaceProxyBuilder> Builders = new Dictionary<string, InterfaceProxyBuilder>();

        public InterfaceProxyFactory()
        {
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName($"OWMLInteraction.Proxies, Version={this.GetType().Assembly.GetName().Version}, Culture=neutral"), AssemblyBuilderAccess.Run);
            this.ModuleBuilder = assemblyBuilder.DefineDynamicModule("OWMLInteraction.Proxies");
        }

        public TInterface CreateProxy<TInterface>(object instance, string sourceModID, string targetModID) where TInterface : class
        {
            if (instance == null)
                throw new InvalidOperationException("Can't proxy access to a null API.");
            if (!typeof(TInterface).IsInterface)
                throw new InvalidOperationException("The proxy type must be an interface, not a class.");

            var targetType = instance.GetType();
            var proxyTypeName = $"OWMLInteraction.Proxies.From<{sourceModID}_{typeof(TInterface).FullName}>_To<{targetModID}_{targetType.FullName}>";
            if (!this.Builders.TryGetValue(proxyTypeName, out InterfaceProxyBuilder builder))
            {
                builder = new InterfaceProxyBuilder(proxyTypeName, this.ModuleBuilder, typeof(TInterface), targetType);
                this.Builders[proxyTypeName] = builder;
            }

            return (TInterface)builder.CreateInstance(instance);
        }
    }
}
