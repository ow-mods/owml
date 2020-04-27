using OWML.Common;
using OWML.ModHelper.Events;
using System.Collections.Generic;
using System.Linq;

namespace OWML.ModHelper.Interaction
{
    public class ModInteraction : IModInteraction
    {
        private readonly List<ModBehaviour> _modList = new List<ModBehaviour>();
        private readonly Dictionary<IModBehaviour, List<IModBehaviour>> _dependantDict = new Dictionary<IModBehaviour, List<IModBehaviour>>();

        public ModInteraction(List<ModBehaviour> list)
        {
            _modList = list;

            foreach (var mod in _modList)
            {
                var dependants = new List<IModBehaviour>();
                foreach (var dependency in _modList)
                {
                    if (dependency.ModHelper.Manifest.Dependencies != new string[] { } && dependency.ModHelper.Manifest.Dependencies.Contains(mod.ModHelper.Manifest.Name))
                    {
                        dependants.Add(dependency);
                    }
                }
                _dependantDict.Add(mod, dependants);
            }
        }

        public IList<IModBehaviour> GetDependants(string dependencyUniqueName)
        {
            return _dependantDict.Where(x => x.Key.ModHelper.Manifest.UniqueName == dependencyUniqueName).FirstOrDefault().Value;
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
            return _modList as IList<IModBehaviour>;
        }

        public bool ModExists(string uniqueName)
        {
            return _modList.Count(m => m.ModHelper.Manifest.UniqueName == uniqueName) != 0;
        }
    }
}
