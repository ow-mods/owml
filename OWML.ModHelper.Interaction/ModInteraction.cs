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
                dependantDict.Add(mod, temp);
            }
        }

        private ModBehaviour GetModFromName(string uniqueName)
        {
            return _modList.First(m => m.ModHelper.Manifest.UniqueName == uniqueName);
        }

        public IList<IModData> GetMods()
        {
            return _finder.GetMods();
        }

        public IList<IModData> GetDependants(string dependencyUniqueName)
        {
            return dependantDict.Where(x => x.Key.Manifest.UniqueName == dependencyUniqueName).FirstOrDefault().Value;
        }

        public bool ModExists(string uniqueName)
        {
            return _finder.GetMods().Count(m => m.Manifest.UniqueName == uniqueName) != 0;
        }

        public T InvokeMethod<T>(string uniqueName, string methodName, params object[] parameters)
        {
            var mod = GetModFromName(uniqueName);
            if (mod.GetType().GetMethod(methodName) == null)
            {
                ModConsole.Instance.WriteLine("Tried to invoke method " + methodName + " in mod " + uniqueName + " but it could not be found.");
                return default;
            }
            return (T)mod.GetType().GetMethod(methodName).Invoke(mod, parameters);
        }

        public void InvokeMethod(string uniqueName, string methodName, params object[] parameters)
        {
            var mod = GetModFromName(uniqueName);
            if (mod.GetType().GetMethod(methodName) == null)
            {
                ModConsole.Instance.WriteLine("Tried to invoke method " + methodName + " in mod " + uniqueName + " but it could not be found.");
                return;
            }
            mod.GetType().GetMethod(methodName).Invoke(mod, parameters);
        }

        public void SetVariableValue(string uniqueName, string variableName, object value)
        {
            var mod = GetModFromName(uniqueName);
            mod.GetType().SetValue(variableName, value);
        }

        public T GetVariableValue<T>(string uniqueName, string variableName)
        {
            var mod = GetModFromName(uniqueName);
            return mod.GetType().GetValue<T>(variableName);
        }
    }
}
