using System.Collections.Generic;

namespace OWML.Common
{
    /// <summary>Per-mod helper for interaction between mods.</summary>
    public interface IModInteraction
    {
        /// <summary>Returns list of mods that depend on the given mod.</summary>
        /// <param name="dependencyUniqueName">The unique name of the mod.</param>
        IList<IModBehaviour> GetDependants(string dependencyUniqueName);

        /// <summary>Returns list of dependencies of the given mod.</summary>
        /// <param name="uniqueName">The unique name of the mod.</param>
        IList<IModBehaviour> GetDependencies(string uniqueName);

        /// <summary>Return the mod that maches the given name.</summary>
        /// <param name="uniqueName">The unique name of the mod.</param>
        IModBehaviour GetMod(string uniqueName);

        /// <summary>Get the API of a given mod.</summary>
        /// <typeparam name="T">The interface through which to access the API.</typeparam>
        /// <param name="uniqueName">The unique name of the mod providing the API.</param>
        T GetApi<T>(string uniqueName) where T : class;

        /// <summary>Returns list of all loaded mods - disabled and enabled.</summary>
        IList<IModBehaviour> GetMods();

        /// <summary>Returns true if a given mod is loaded - *not* if the mod is enabled/disabled.</summary>
        /// <param name="uniqueName">The unique name of the mod.</param>
        bool ModExists(string uniqueName);
    }
}
