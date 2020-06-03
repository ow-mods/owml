using System.Collections.Generic;

namespace OWML.Common
{
    /// <summary>
    /// The mod behaviour.
    /// </summary>
    public interface IModBehaviour
    {
        /// <summary>
        /// The ModHelper attached to this mod.
        /// </summary>
        IModHelper ModHelper { get; }

        /// <summary>
        /// The API attached to this mod.
        /// </summary>
        object Api { get; }

        /// <summary>
        /// Called whenver the config file is updated.
        /// </summary>
        /// <param name="config">The updated mod config.</param>
        void Configure(IModConfig config);

        /// <summary>Returns list of mods that depend on the current mod.</summary>
        IList<IModBehaviour> GetDependants();

        /// <summary>Returns dependencies of current mod.</summary>
        IList<IModBehaviour> GetDependencies();

        /// <summary>
        /// Called on mod load to get API.
        /// </summary>
        object GetApi();
    }
}
