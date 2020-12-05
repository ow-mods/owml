using System.Collections.Generic;
using System.IO;
using OWML.Common;
using OWML.Common.Enums;
using OWML.Common.Interfaces;
using OWML.Common.Models;
using OWML.ModHelper;

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

        public List<IModData> GetMods()
        {
            if (!Directory.Exists(_config.ModsPath))
            {
                _console.WriteLine("Warning - Mods folder not found!", MessageType.Warning);
                return new List<IModData>();
            }
            var manifestFilenames = Directory.GetFiles(_config.ModsPath, Constants.ModManifestFileName, SearchOption.AllDirectories);
            var mods = new List<IModData>();
            foreach (var manifestFilename in manifestFilenames)
            {
                var manifest = JsonHelper.LoadJsonObject<ModManifest>(manifestFilename);
                manifest.ModFolderPath = manifestFilename.Substring(0, manifestFilename.IndexOf(Constants.ModManifestFileName));
                var modData = InitModData(manifest);
                mods.Add(modData);
            }
            return mods;
        }

        private IModData InitModData(IModManifest manifest)
        {
            var storage = new ModStorage(manifest);
            var config = storage.Load<ModConfig>(Constants.ModConfigFileName);
            var defaultConfig = storage.Load<ModConfig>(Constants.ModDefaultConfigFileName);
            return new ModData(manifest, config, defaultConfig, storage);
        }
    }
}
