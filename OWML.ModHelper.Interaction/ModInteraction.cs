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
        private readonly Dictionary<IModData, List<IModData>> dependantDict = new Dictionary<IModData, List<IModData>>();

        public ModInteraction(List<ModBehaviour> list, IModFinder finder)
        {
            _modList = list;
            _finder = finder;
            // Creates dictionary of key [Dependency] and value [Mods that depend on that dependency]
            foreach (var mod in _finder.GetMods())
            {
                List<IModData> temp = new List<IModData>();
                foreach (var mod2 in _finder.GetMods())
                {
                    if (mod2.Manifest.Dependencies != null)
                    {
                        if (mod2.Manifest.Dependencies.Contains(mod.Manifest.Name))
                        {
                            temp.Add(mod2);
                        }
                    }
                }
                dependantDict.Add(mod, temp);
            }
        }

        public IList<IModData> GetMods()
        {
            return _finder.GetMods();
        }

        public IList<IModData> GetDependants(string dependencyName)
        {
            return dependantDict.Where(x => x.Key.Manifest.UniqueName == dependencyName).FirstOrDefault().Value;
        }

        public bool ModExists(string uniqueName)
        {
            return _finder.GetMods().Where(m => m.Manifest.UniqueName == uniqueName).Count() != 0;
        }

        public T InvokeMethod<T>(string uniqueName, string methodName, params object[] parameters)
        {
            var mod = _modList.Where(m => m.ModHelper.Manifest.UniqueName == uniqueName).First();
            return (T)mod.GetType().GetMethod(methodName).Invoke(mod, parameters);
        }

        public void InvokeMethod(string uniqueName, string methodName, params object[] parameters)
        {
            var mod = _modList.Where(m => m.ModHelper.Manifest.UniqueName == uniqueName).First();
            mod.GetType().GetMethod(methodName).Invoke(mod, parameters);
        }

        public void SetVariableValue(string uniqueName, string variableName, object value)
        {
            var mod = _modList.Where(m => m.ModHelper.Manifest.UniqueName == uniqueName).First();
            mod.GetType().SetValue(variableName, value);
        }

        public T GetVariableValue<T>(string uniqueName, string variableName)
        {
            var mod = _modList.Where(m => m.ModHelper.Manifest.UniqueName == uniqueName).First();
            return mod.GetType().GetValue<T>(variableName);
        }
    }
}
