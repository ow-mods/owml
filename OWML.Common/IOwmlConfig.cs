namespace OWML.Common
{
    public interface IOwmlConfig
    {
        string GamePath { get; set; }
        string ManagedPath { get; }
        string PluginsPath { get; }
        string DataPath { get; }
        string OWMLPath { get; }
        string ModsPath { get; }
        string OutputFilePath { get; }
        string LogFilePath { get; }
        bool Verbose { get; set; }
        bool BlockInput { get; set; }
    }
}
