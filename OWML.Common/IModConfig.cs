namespace OWML.Common
{
    public interface IModConfig
    {
        string GamePath { get; }
        string ManagedPath { get; }
        string OWMLPath { get; }
        string ModsPath { get; }
        string OutputFilePath { get; }
        string LogFilePath { get; }
        bool Verbose { get; }
    }
}
