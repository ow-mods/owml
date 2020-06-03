namespace OWML.Common
{
    /// <summary>Contains the config and path data for OWML.</summary>
    public interface IOwmlConfig
    {
        /// <summary>The path of the game exe.</summary>
        string GamePath { get; set; }

        /// <summary>The path of the managed game data..</summary>
        string ManagedPath { get; }

        /// <summary>The path of the game plugins.</summary>
        string PluginsPath { get; }

        /// <summary>The path of the game data.</summary>
        string DataPath { get; }

        /// <summary>The path OWML is located in.</summary>
        string OWMLPath { get; }

        /// <summary>The location of the mods.</summary>
        string ModsPath { get; }

        /// <summary>The path of OWML's output file.</summary>
        string OutputFilePath { get; }

        /// <summary>The path of OWML's log file.</summary>
        string LogFilePath { get; }

        /// <summary>Is OWML in verbose mode?.</summary>
        bool Verbose { get; }
    }
}
