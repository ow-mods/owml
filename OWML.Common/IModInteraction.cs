using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OWML.Common
{
    public interface IModInteraction
    {
        IList<IModData> GetMods();
        bool ModExists(string uniqueName);
        T InvokeMethod<T>(string uniqueName, string methodName, params object[] parameters);
        void SetVariableValue(string uniqueName, string variableName, object value);
    }
}
