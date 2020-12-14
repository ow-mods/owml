using System.Collections.Generic;
using System.IO;
using System.Linq;
using OWML.Common;
using OWML.ModHelper;
using OWML.Utils;

namespace OWML.ModLoader
{
	public class ModFinder : IModFinder
	{
		private readonly IOwmlConfig _config;
		private readonly IModConsole _console;

		public ModFinder(IOwmlConfig config, IModConsole console)
		{
			_config = config;
			_console = console;
		}

		public IList<IModData> GetMods()
		{
			if (!Directory.Exists(_config.ModsPath))
			{
				_console.WriteLine("Warning - Mods folder not found!", MessageType.Warning);
				return new List<IModData>();
			}

			var manifestFilenames = Directory.GetFiles(_config.ModsPath, Constants.ModManifestFileName, SearchOption.AllDirectories);

			return manifestFilenames.Select(GetMod).ToList();
		}

		private IModData GetMod(string manifestFilename)
		{
			var manifest = JsonHelper.LoadJsonObject<ModManifest>(manifestFilename);
			manifest.ModFolderPath = manifestFilename.Substring(0, manifestFilename.IndexOf(Constants.ModManifestFileName));
			return InitModData(manifest);
		}

		private IModData InitModData(IModManifest manifest) // todo DI?
		{
			var storage = new ModStorage(manifest);
			var config = storage.Load<ModConfig>(Constants.ModConfigFileName);
			var defaultConfig = storage.Load<ModConfig>(Constants.ModDefaultConfigFileName) ?? new ModConfig();
			return new ModData(manifest, config, defaultConfig, storage);
		}
	}
}
