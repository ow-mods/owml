using System.Collections.Generic;

namespace OWML.Common
{
    /// <summary>
    /// Contains manifest data for a mod.
    /// </summary>
    public interface IModManifest
    {
        /// <summary>The name of the dll file.</summary>
        string Filename { get; }

        /// <summary>The name of the mod author.</summary>
        string Author { get; }

        /// <summary>The name of the mod.</summary>
        string Name { get; }

        /// <summary>The version of the mod.</summary>
        string Version { get; }

        /// <summary>The version of OWML the mod was made for.</summary>
        string OWMLVersion { get; }

        /// <summary>The filepath of the mod dll. (ModFolderPath + Filename)</summary>
        string AssemblyPath { get; }

        /// <summary>The unique name of the mod. (Author + Name)</summary>
        string UniqueName { get; }

        /// <summary>The filepath of the mod.</summary>
        string ModFolderPath { get; set; }

        Dictionary<string, string> AppIds { get; }

        /// <summary>The list of dependencies this mod needs.</summary>
        string[] Dependencies { get; }

        /// <summary>Is the mod a priority load mod?</summary>
        bool PriorityLoad { get; }
    }
}
