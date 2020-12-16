using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using OWML.Common;

// Adapted from SMAPI code : https://github.com/Pathoschild/SMAPI/tree/c4a82418ac8b09a6965052f5c9173928457fba52/src/SMAPI/Framework/Reflection

namespace OWML.ModHelper.Interaction
{
	public class InterfaceProxyFactory : IInterfaceProxyFactory
	{
		private readonly ModuleBuilder _moduleBuilder;
		private readonly IDictionary<string, InterfaceProxyBuilder> _builders = new Dictionary<string, InterfaceProxyBuilder>();

		public InterfaceProxyFactory()
		{
			var assemblyName = new AssemblyName($"OWMLInteraction.Proxies, Version={GetType().Assembly.GetName().Version}, Culture=neutral");
			var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
			_moduleBuilder = assemblyBuilder.DefineDynamicModule("OWMLInteraction.Proxies");
		}

		public TInterface CreateProxy<TInterface>(object instance, string sourceModName, string targetModName) where TInterface : class
		{
			if (instance == null)
			{
				throw new InvalidOperationException("Can't proxy access to a null API.");
			}
			if (!typeof(TInterface).IsInterface)
			{
				throw new InvalidOperationException("The proxy type must be an interface, not a class.");
			}

			var targetType = instance.GetType();

			var proxyTypeName = $"OWMLInteraction.Proxies.From<{sourceModName}_{typeof(TInterface).FullName}>_To<{targetModName}_{targetType.FullName}>";
			if (!_builders.TryGetValue(proxyTypeName, out var builder))
			{
				builder = new InterfaceProxyBuilder(proxyTypeName, _moduleBuilder, typeof(TInterface), targetType);
				_builders[proxyTypeName] = builder;
			}

			return (TInterface)builder.CreateInstance(instance);
		}
	}
}
