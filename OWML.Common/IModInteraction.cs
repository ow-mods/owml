using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OWML.Common
{
    public interface IModInteraction
    {
        IList<IModData> GetMods();
        IList<IModData> GetDependants(string dependencyName);
        bool ModExists(string uniqueName);
        T InvokeMethod<T>(string uniqueName, string methodName, params object[] parameters);
        void InvokeMethod(string uniqueName, string methodName, params object[] parameters);
        void SetVariableValue(string uniqueName, string variableName, object value);
        T GetVariableValue<T>(string uniqueName, string variableName);
    }
}
