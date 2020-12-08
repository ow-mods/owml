namespace OWML.Common.Interfaces
{
    public interface IOwmlConfig
    {
        string GamePath { get; set; }

        string ManagedPath { get; }

        string PluginsPath { get; }

        string DataPath { get; }

        string OWMLPath { get; set; }

        string ModsPath { get; }

        string LogsPath { get; }

        bool BlockInput { get; set; }

        int SocketPort { get; set; }
    }
}
