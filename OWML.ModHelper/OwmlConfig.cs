using Newtonsoft.Json;
using OWML.Common.Interfaces;

namespace OWML.ModHelper
{
    public class OwmlConfig : IOwmlConfig
    {
        [JsonProperty("gamePath")]
        public string GamePath { get; set; }

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
        public string ModsPath => $"{OWMLPath}Mods";

        [JsonProperty("socketPort")]
        public int SocketPort { get; set; }
    }
}
