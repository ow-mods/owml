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

        /// <summary>Construct an instance.</summary>
        public InterfaceProxyFactory()
        {
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName($"OWMLInteraction.Proxies, Version={this.GetType().Assembly.GetName().Version}, Culture=neutral"), AssemblyBuilderAccess.Run);
            this.ModuleBuilder = assemblyBuilder.DefineDynamicModule("OWMLInteraction.Proxies");
        }

        /// <summary>Create an API proxy.</summary>
        /// <typeparam name="TInterface">The interface through which to access the API.</typeparam>
        /// <param name="instance">The API instance to access.</param>
        /// <param name="sourceModID">The unique ID of the mod consuming the API.</param>
        /// <param name="targetModID">The unique ID of the mod providing the API.</param>
        public TInterface CreateProxy<TInterface>(object instance, string sourceModID, string targetModID)
            where TInterface : class
        {
            // validate
            if (instance == null)
                throw new InvalidOperationException("Can't proxy access to a null API.");
            if (!typeof(TInterface).IsInterface)
                throw new InvalidOperationException("The proxy type must be an interface, not a class.");

            // get proxy type
            Type targetType = instance.GetType();
            string proxyTypeName = $"OWMLInteraction.Proxies.From<{sourceModID}_{typeof(TInterface).FullName}>_To<{targetModID}_{targetType.FullName}>";
            if (!this.Builders.TryGetValue(proxyTypeName, out InterfaceProxyBuilder builder))
            {
                builder = new InterfaceProxyBuilder(proxyTypeName, this.ModuleBuilder, typeof(TInterface), targetType);
                this.Builders[proxyTypeName] = builder;
            }

            // create instance
            return (TInterface)builder.CreateInstance(instance);
        }
    }
}
