using Newtonsoft.Json;
using OWML.Common;

namespace OWML.ModHelper
{
    public class OwmlConfig : IOwmlConfig
    {
        [JsonProperty("gamePath")]
        public string GamePath { get; set; }

        [JsonProperty("verbose")]
        public bool Verbose { get; set; }

        [JsonProperty("combinationsBlockInput")]
        public bool BlockInput { get; set; }

        [JsonIgnore]
        public string DataPath => $"{GamePath}/OuterWilds_Data";

        [JsonIgnore]
        public string ManagedPath => $"{DataPath}/Managed";

        [JsonIgnore]
        public string PluginsPath => $"{DataPath}/Plugins";

        [JsonProperty("owmlPath")]
        public string OWMLPath { get; set; }

        [JsonIgnore]
        public string LogFilePath => $"{OWMLPath}Logs/OWML.Log.txt";

        [JsonIgnore]
        public string OutputFilePath => $"{OWMLPath}Logs/OWML.Output.txt";

        [JsonIgnore]
        public string ModsPath => $"{OWMLPath}Mods";
    }
}
