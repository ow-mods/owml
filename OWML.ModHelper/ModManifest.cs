using Newtonsoft.Json;
using OWML.Common;

namespace OWML.ModHelper
{
    public class ModManifest : IModManifest
    {
        [JsonProperty("filename")]
        public string Filename { get; private set; }

        [JsonProperty("author")]
        public string Author { get; private set; }

        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("uniqueName")]
        public string UniqueName { get; private set; }

        [JsonProperty("version")]
        public string Version { get; private set; }

        [JsonProperty("owmlVersion")]
        public string OWMLVersion { get; private set; }

        [JsonProperty("enabled")]
        public bool Enabled { get; private set; }

        [JsonIgnore]
        public string ModFolderPath { get; set; }

        [JsonIgnore]
        public string AssemblyPath => ModFolderPath + Filename;
    }
}
