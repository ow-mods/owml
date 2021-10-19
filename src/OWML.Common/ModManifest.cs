using Newtonsoft.Json;

namespace OWML.Common
{
	public class ModManifest : IModManifest
	{
		[JsonProperty("filename")]
		public string Filename { get; private set; }

		[JsonProperty("patcher")]
		public string Patcher { get; private set; }

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

		[JsonIgnore]
		public string AssemblyPath => ModFolderPath + Filename;

		[JsonIgnore]
		public string PatcherPath => ModFolderPath + Patcher;

		[JsonProperty("minGameVersion")]
		public string MinGameVersion { get; private set; } = "";

		[JsonProperty("maxGameVersion")]
		public string MaxGameVersion { get; private set; } = "";
	}
}
