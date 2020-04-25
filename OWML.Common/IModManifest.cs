using System.Collections.Generic;

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
        Dictionary<string, string> AppIds { get; }
        string Dependency { get; }
    }
}
