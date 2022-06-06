﻿using System;
using System.Collections.Generic;
using System.Linq;
using OWML.Common;

namespace OWML.ModHelper.Interaction
{
	public class ModInteraction : IModInteraction
	{
		private readonly IList<IModBehaviour> _modList;
		private readonly IInterfaceProxyFactory _proxyFactory;
		private readonly IModManifest _manifest;

		private Dictionary<string, List<IModBehaviour>> _dependantDict = new();
		private Dictionary<string, List<IModBehaviour>> _dependencyDict = new();

		public ModInteraction(IList<IModBehaviour> modList, IInterfaceProxyFactory proxyFactory, IModManifest manifest)
		{
			_manifest = manifest;
			_proxyFactory = proxyFactory;
			_modList = modList;

			RegenerateDictionaries();
		}

		private void RegenerateDictionaries()
		{
			_dependantDict = new Dictionary<string, List<IModBehaviour>>();
			_dependencyDict = new Dictionary<string, List<IModBehaviour>>();
			foreach (var mod in _modList)
			{
				var dependants = new List<IModBehaviour>();
				var dependencies = new List<IModBehaviour>();
				foreach (var dependency in _modList)
				{
					if (dependency.ModHelper.Manifest.Dependencies.Contains(mod.ModHelper.Manifest.UniqueName))
					{
						dependants.Add(dependency);
					}

					if (mod.ModHelper.Manifest.Dependencies.Contains(dependency.ModHelper.Manifest.UniqueName))
					{
						dependencies.Add(dependency);
					}
				}
				_dependantDict[mod.ModHelper.Manifest.UniqueName] = dependants;
				_dependencyDict[mod.ModHelper.Manifest.UniqueName] = dependencies;
			}
		}

		public IList<IModBehaviour> GetDependants(string dependencyUniqueName)
		{
			if (_dependantDict.Count != _modList.Count)
			{
				RegenerateDictionaries();
			}
			return _dependantDict[dependencyUniqueName];
		}

		public IList<IModBehaviour> GetDependencies(string uniqueName)
		{
			if (_dependantDict.Count != _modList.Count)
			{
				RegenerateDictionaries();
			}
			return _dependencyDict[uniqueName];
		}

		public IModBehaviour GetMod(string uniqueName) => 
			_modList.FirstOrDefault(m => m.ModHelper.Manifest.UniqueName == uniqueName);

		private object GetApi(string uniqueName)
		{
			var mod = GetMod(uniqueName);
			return mod == default ? default : mod.Api;
		}

		public TInterface GetModApi<TInterface>(string uniqueName, bool throwException = true) where TInterface : class
		{
			var api = GetApi(uniqueName);

			if (api == default)
			{
				if (throwException)
				{
					throw new Exception($"{uniqueName} is not installed / enabled.");
				}

				return default;
			}

			return api switch
			{
				null => null,
				TInterface inter => inter,
				_ => _proxyFactory.CreateProxy<TInterface>(api, _manifest.UniqueName, uniqueName)
			};
		}

		public IList<IModBehaviour> GetMods() => 
			_modList;

		public bool ModExists(string uniqueName) => 
			_modList.Any(m => m.ModHelper.Manifest.UniqueName == uniqueName);
	}
}
