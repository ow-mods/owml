using OWML.Common;
using OWML.ModHelper.Events;
using System.Collections.Generic;
using System.Linq;

namespace OWML.ModHelper.Interaction
{
    public class ModInteraction : IModInteraction
    {
        private readonly List<ModBehaviour> _modList = new List<ModBehaviour>();
        private readonly IModFinder _finder;
        private readonly Dictionary<IModData, List<IModData>> _dependantDict = new Dictionary<IModData, List<IModData>>();

        public ModInteraction(List<ModBehaviour> list, IModFinder finder)
        {
            _modList = list;
            _finder = finder;

            foreach (var mod in _finder.GetMods())
            {
                List<IModData> temp = new List<IModData>();
                foreach (var mod2 in _finder.GetMods())
                {
                    if (mod2.Manifest.Dependencies != null && mod2.Manifest.Dependencies.Contains(mod.Manifest.Name))
                    {
                        temp.Add(mod2);
                    }
                }
                _dependantDict.Add(mod, temp);
            }
        }

        public List<IModData> GetDependants(string dependencyUniqueName)
        {
            return _dependantDict.Where(x => x.Key.Manifest.UniqueName == dependencyUniqueName).FirstOrDefault().Value;
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

        public IList<IModData> GetMods()
        {
            return _finder.GetMods();
        }

        public bool ModExists(string uniqueName)
        {
            return _finder.GetMods().Count(m => m.Manifest.UniqueName == uniqueName) != 0;
        }
    }
}
