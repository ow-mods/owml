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

		[JsonIgnore]
		public string DataPath => $"{GamePath}/OuterWilds_Data";

		[JsonIgnore]
		public string ExePath => $"{GamePath}/OuterWilds.exe";

		[JsonIgnore]
		public string ManagedPath => $"{DataPath}/Managed";

		[JsonIgnore]
		public string PluginsPath => $"{DataPath}/Plugins";

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
