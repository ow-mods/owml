using OWML.Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OWML.ModHelper.Interaction
{
    public class ModInteraction : IModInteraction
    {
        private readonly IList<IModBehaviour> _modList;
        private Dictionary<string, List<IModBehaviour>> _dependantDict = new Dictionary<string, List<IModBehaviour>>();
        private Dictionary<string, List<IModBehaviour>> _dependencyDict = new Dictionary<string, List<IModBehaviour>>();

        public ModInteraction(IList<IModBehaviour> list)
        {
            _modList = list;

            RegenDicts();
        }

        private void RegenDicts()
        {
            _dependantDict = new Dictionary<string, List<IModBehaviour>>();
            _dependencyDict = new Dictionary<string, List<IModBehaviour>>();
            foreach (var mod in _modList)
            {
                var dependants = new List<IModBehaviour>();
                var dependencies = new List<IModBehaviour>();
                foreach (var dependency in _modList)
                { 
                    if (dependency.ModHelper.Manifest.Dependencies.Any() && dependency.ModHelper.Manifest.Dependencies.Contains(mod.ModHelper.Manifest.UniqueName))
                    {
                        dependants.Add(dependency);
                    }

                    if (mod.ModHelper.Manifest.Dependencies.Contains(dependency.ModHelper.Manifest.UniqueName))
                    {
                        dependencies.Add(dependency);
                    }
                }
                _dependantDict.Add(mod.ModHelper.Manifest.UniqueName, dependants);
                _dependencyDict.Add(mod.ModHelper.Manifest.UniqueName, dependencies);
            }
        }

        public IList<IModBehaviour> GetDependants(string dependencyUniqueName)
        {
            RegenDicts();
            return _dependantDict[dependencyUniqueName];
        }

        public IList<IModBehaviour> GetDependencies(string uniqueName)
        {
            if (_dependantDict.Count != _modList.Count)
            {
                RegenDicts();
            }
            return _dependencyDict[uniqueName];
        }

        public IModBehaviour GetMod(string uniqueName)
        {
            return _modList.First(m => m.ModHelper.Manifest.UniqueName == uniqueName);
        }

        public T GetMod<T>(string uniqueName) where T : IModBehaviour
        {
            var mod = GetMod(uniqueName);
            return (T)mod;
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
