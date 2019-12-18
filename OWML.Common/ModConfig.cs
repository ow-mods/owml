using Newtonsoft.Json;

namespace OWML.Common
{
    public class ModConfig : IModConfig
    {
        [JsonProperty("gamePath")]
        public string GamePath { get; set; }
        
        [JsonIgnore]
        public string ManagedPath => $"{GamePath}/OuterWilds_Data/Managed";

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
