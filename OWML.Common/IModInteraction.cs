using System.Collections.Generic;

namespace OWML.Common
{
    public interface IModInteraction
    {
        IList<IModBehaviour> GetMods();
        IList<IModBehaviour> GetDependants(string dependencyUniqueName);
        IList<IModBehaviour> GetDependencies(string uniqueName);
        IModBehaviour GetMod(string uniqueName);
        T GetApi<T>(string uniqueName) where T : class;
        bool ModExists(string uniqueName);
    }
}
