using System;

namespace OWML.Common
{
    public interface IModManifest
    {
        string Filename { get; }
        string Author { get; }
        string Name { get; }
        string Version { get; }
        string OWMLVersion { get; }
        string AssemblyPath { get; }
        string UniqueName { get; }
        string ModFolderPath { get; set; }

        [Obsolete("Use Enabled in ModConfig instead")]
        bool Enabled { get; }
    }
}
