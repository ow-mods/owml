using OWML.Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OWML.ModHelper.Interaction
{
    public class ModInteraction : IModInteraction
    {
        private readonly IList<ModBehaviour> _modList;
        private Dictionary<ModBehaviour, List<ModBehaviour>> _dependantDict = new Dictionary<ModBehaviour, List<ModBehaviour>>();
        private Dictionary<ModBehaviour, List<ModBehaviour>> _dependencyDict = new Dictionary<ModBehaviour, List<ModBehaviour>>();

        public ModInteraction(IList<ModBehaviour> list)
        {
            _modList = list;

            RegenDicts();
        }

        private void RegenDicts()
        {
            _dependantDict = new Dictionary<ModBehaviour, List<ModBehaviour>>();
            _dependencyDict = new Dictionary<ModBehaviour, List<ModBehaviour>>();
            foreach (var mod in _modList)
            {
                var dependants = new List<ModBehaviour>();
                var dependencies = new List<ModBehaviour>();
                foreach (var dependency in _modList)
                { 
                    if (dependency.ModHelper.Manifest.Dependencies != new string[] { } && dependency.ModHelper.Manifest.Dependencies.Contains(mod.ModHelper.Manifest.UniqueName))
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
            return _dependantDict.Where(x => x.Key.ModHelper.Manifest.UniqueName == dependencyUniqueName).FirstOrDefault().Value.Cast<IModBehaviour>().ToList();
        }

        public IList<IModBehaviour> GetDependencies(string uniqueName)
        {
            RegenDicts();
            return _dependencyDict.Where(x => x.Key.ModHelper.Manifest.UniqueName == uniqueName).FirstOrDefault().Value.Cast<IModBehaviour>().ToList();
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
            return _modList.Cast<IModBehaviour>().ToList();
        }

        public bool ModExists(string uniqueName)
        {
            return _modList.Count(m => m.ModHelper.Manifest.UniqueName == uniqueName) != 0;
        }
    }
}
