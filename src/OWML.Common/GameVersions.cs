using Newtonsoft.Json;
using OWML.Common.Interfaces;

namespace OWML.Common
{
	public class GameVersions : IGameVersions
	{
		[JsonProperty("steam")]
		public string Steam { get; private set; }

		[JsonProperty("epic")]
		public string Epic { get; private set; }

		[JsonProperty("gamepass")]
		public string Gamepass { get; private set; }
	}
}
