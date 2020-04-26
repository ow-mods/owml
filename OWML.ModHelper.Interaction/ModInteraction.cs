using OWML.Common;
using OWML.ModHelper.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OWML.ModHelper.Interaction
{
    public class ModInteraction : IModInteraction
    {
        private readonly List<ModBehaviour> _modList = new List<ModBehaviour>();
        private readonly IModFinder _finder;
        public ModInteraction(List<ModBehaviour> list, IModFinder finder)
        {
            _modList = list;
            _finder = finder;
        }

        public IList<IModData> GetMods()
        {
            return _finder.GetMods();
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

        public void SetVariableValue(string uniqueName, string variableName, object value)
        {
            var mod = _modList.Where(m => m.ModHelper.Manifest.UniqueName == uniqueName).First();
            mod.GetType().SetValue(variableName, value);
        }
    }
}
