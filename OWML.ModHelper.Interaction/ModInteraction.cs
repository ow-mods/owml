using OWML.Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OWML.ModHelper.Interaction
{
    public class ModInteraction : IModInteraction
    {
        private readonly IList<IModBehaviour> _modList;
        private Dictionary<IModBehaviour, List<IModBehaviour>> _dependantDict = new Dictionary<IModBehaviour, List<IModBehaviour>>();
        private Dictionary<IModBehaviour, List<IModBehaviour>> _dependencyDict = new Dictionary<IModBehaviour, List<IModBehaviour>>();

        public ModInteraction(IList<IModBehaviour> list)
        {
            _modList = list;

            RegenDicts();
        }

        private void RegenDicts()
        {
            _dependantDict = new Dictionary<IModBehaviour, List<IModBehaviour>>();
            _dependencyDict = new Dictionary<IModBehaviour, List<IModBehaviour>>();
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
                _dependantDict.Add(mod, dependants);
                _dependencyDict.Add(mod, dependencies);
            }
        }

        public IList<IModBehaviour> GetDependants(string dependencyUniqueName)
        {
            RegenDicts();
            return _dependantDict.Where(x => x.Key.ModHelper.Manifest.UniqueName == dependencyUniqueName).FirstOrDefault().Value;
        }

        public IList<IModBehaviour> GetDependencies(string uniqueName)
        {
            RegenDicts();
            return _dependencyDict.Where(x => x.Key.ModHelper.Manifest.UniqueName == uniqueName).FirstOrDefault().Value;
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
            return _modList.Count(m => m.ModHelper.Manifest.UniqueName == uniqueName) != 0;
        }
    }
}
