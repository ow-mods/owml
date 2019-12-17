using Newtonsoft.Json;

namespace OWML.Common
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

        [JsonProperty("enabled")]
        public bool Enabled { get; private set; }

        [JsonIgnore]
        public string FolderPath { get; set; }

        [JsonIgnore]
        public string AssemblyPath => FolderPath + Filename;
    }
}
