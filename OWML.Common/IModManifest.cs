namespace OWML.Common
{
    public interface IModManifest
    {
        string Filename { get; }
        string Author { get; }
        string Name { get; }
        string Version { get; }
        string AssemblyPath { get; }
        string UniqueName { get; }
        string FolderPath { get; set; }
        bool Enabled { get; }
    }
}
