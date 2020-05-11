using System.Collections.Generic;

namespace OWML.Common
{
    public interface IModInteraction
    {
        IList<IModBehaviour> GetMods();
        IList<IModBehaviour> GetDependants(string dependencyUniqueName);
        IList<IModBehaviour> GetDependencies(string uniqueName);
        IModBehaviour GetMod(string uniqueName);
        T GetMod<T>(string uniqueName) where T : IModBehaviour;
        bool ModExists(string uniqueName);
    }
}
