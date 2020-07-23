using System.Collections.Generic;
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

        [JsonProperty("dependencies")]
        public string[] Dependencies { get; private set; } = { };

        [JsonProperty("priorityLoad")]
        public bool PriorityLoad { get; private set; }

        [JsonIgnore]
        public string ModFolderPath { get; set; }

        [JsonProperty("appIds")]
        public Dictionary<string, string> AppIds { get; private set; }

        [JsonProperty("requireVR")]
        public bool RequireVR { get; set; }

        [JsonIgnore]
        public string AssemblyPath => ModFolderPath + Filename;
    }
}
