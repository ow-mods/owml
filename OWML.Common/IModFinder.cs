using System.Collections.Generic;

namespace OWML.Common
{
    /// <summary>
    /// Helper for finding mods.
    /// </summary>
    public interface IModFinder
    {
        /// <summary>
        /// Returns a list of mods in IModData form.
        /// </summary>
        IList<IModData> GetMods();
    }
}
