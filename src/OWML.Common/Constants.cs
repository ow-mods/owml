using System.Runtime.CompilerServices;

// make everything in OWML.Common visible to these namespaces
// this is in this file just because it's the first one that comes up in VS :P
[assembly: InternalsVisibleTo("OWML.ModHelper.Menus")]
[assembly: InternalsVisibleTo("OWML.ModLoader")]

namespace OWML.Common
{
	public class Constants
	{
		public const string OwmlTitle = "OWML";

		public const string ConsolePortArgument = "consolePort";

		public const string OwmlConfigFileName = "OWML.Config.json";

		public const string OwmlDefaultConfigFileName = "OWML.DefaultConfig.json";

		public const string OwmlManifestFileName = "OWML.Manifest.json";

		public const string LocalAddress = "127.0.0.1";

		public const string ModConfigFileName = "config.json";

		public const string ModDefaultConfigFileName = "default-config.json";

		public const string ModManifestFileName = "manifest.json";

		public const string GameVersionsFileName = "game-versions.json";
	}
}
