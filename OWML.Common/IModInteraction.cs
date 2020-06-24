using System.Collections.Generic;

namespace OWML.Common
{
    public interface IModInteraction
    {
        IList<IModBehaviour> GetMods();
        IList<IModBehaviour> GetDependants(string dependencyUniqueName);
        IList<IModBehaviour> GetDependencies(string uniqueName);
        IModBehaviour GetMod(string uniqueName);
        TInterface GetModApi<TInterface>(string uniqueName) where TInterface : class;
        bool ModExists(string uniqueName);
    }
}
