using System.Collections.Generic;
using System.Linq;
using OWML.Common.Interfaces;

namespace OWML.ModHelper.Interaction
{
    public class ModInteraction : IModInteraction
    {
        private readonly IList<IModBehaviour> _modList;
        private readonly InterfaceProxyFactory _proxyFactory;
        private readonly IModManifest _manifest;
        private Dictionary<string, List<IModBehaviour>> _dependantDict = new Dictionary<string, List<IModBehaviour>>();
        private Dictionary<string, List<IModBehaviour>> _dependencyDict = new Dictionary<string, List<IModBehaviour>>();

        public ModInteraction(IList<IModBehaviour> list, InterfaceProxyFactory proxyFactory, IModManifest manifest)
        {
            _modList = list;
            _manifest = manifest;
            _proxyFactory = proxyFactory;
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

        public IModBehaviour GetMod(string uniqueName)
        {
            return _modList.First(m => m.ModHelper.Manifest.UniqueName == uniqueName);
        }

        private object GetApi(string uniqueName)
        {
            var mod = GetMod(uniqueName);
            return mod.Api;
        }

        public TInterface GetModApi<TInterface>(string uniqueName) where TInterface : class
        {
            var inter = GetApi(uniqueName);
            if (inter == null)
            {
                return null;
            }

            if (inter is TInterface castInter)
            {
                return castInter;
            }

            return _proxyFactory.CreateProxy<TInterface>(inter, _manifest.UniqueName, uniqueName);
        }

        public IList<IModBehaviour> GetMods()
        {
            return _modList;
        }

        public bool ModExists(string uniqueName)
        {
            return _modList.Any(m => m.ModHelper.Manifest.UniqueName == uniqueName);
        }
    }
}
