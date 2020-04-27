using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OWML.Common
{
    public interface IModInteraction
    {
        IList<IModData> GetMods();
        List<IModData> GetDependants(string dependencyUniqueName);
        IModBehaviour GetMod(string uniqueName);
        T GetMod<T>(string uniqueName) where T : IModBehaviour;
        bool ModExists(string uniqueName);
    }
}
