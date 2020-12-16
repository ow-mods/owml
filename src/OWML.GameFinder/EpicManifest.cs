using Newtonsoft.Json;

namespace OWML.GameFinder
{
	public class EpicManifest
	{
		[JsonProperty("InstallLocation")]
		public string InstallLocation { get; set; }
	}
}