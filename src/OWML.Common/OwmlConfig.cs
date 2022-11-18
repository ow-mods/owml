using System.IO;
using Newtonsoft.Json;

namespace OWML.Common
{
	public class OwmlConfig : IOwmlConfig
	{
		[JsonProperty("gamePath")]
		public string GamePath { get; set; }

		[JsonProperty("debugMode")]
		public bool DebugMode { get; set; }

		[JsonProperty("forceExe")]
		public bool ForceExe { get; set; }

		[JsonProperty("incrementalGC")]
		public bool IncrementalGC { get; set; }

		[JsonIgnore]
		public bool IsSpaced => Directory.Exists(Path.Combine(GamePath, "Outer Wilds_Data"));

		[JsonIgnore]
		public string DataPath => Path.Combine(GamePath, IsSpaced ? "Outer Wilds_Data" : "OuterWilds_Data");

		[JsonIgnore]
		public string ExePath => Path.Combine(GamePath, IsSpaced ? "Outer Wilds.exe" : "OuterWilds.exe");

		[JsonIgnore]
		public string ManagedPath => Path.Combine(DataPath, "Managed");

		[JsonIgnore]
		public string PluginsPath => Path.Combine(DataPath, "Plugins");

		[JsonProperty("owmlPath")]
		public string OWMLPath { get; set; }

		[JsonIgnore]
		public string ModsPath => $"{OWMLPath}Mods";

		[JsonIgnore]
		public string LogsPath => $"{OWMLPath}Logs";

		[JsonProperty("socketPort")]
		public int SocketPort { get; set; }
	}
}
