using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using OWML.Common;
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

        public IList<IModData> GetMods()
        {
            if (!Directory.Exists(_config.ModsPath))
            {
                _console.WriteLine("Warning: Mods folder not found!");
                return new List<IModData>();
            }
            var manifestFilenames = Directory.GetFiles(_config.ModsPath, Constants.ModManifestFileName, SearchOption.AllDirectories);
            var mods = new List<IModData>();
            foreach (var manifestFilename in manifestFilenames)
            {
                var json = File.ReadAllText(manifestFilename);
                var manifest = JsonConvert.DeserializeObject<ModManifest>(json);
                manifest.ModFolderPath = manifestFilename.Substring(0, manifestFilename.IndexOf(Constants.ModManifestFileName));
                var modData = GetModData(manifest);
                mods.Add(modData);
            }
            return mods;
        }

        private IModData GetModData(IModManifest manifest)
        {
            var storage = new ModStorage(manifest);
            var config = storage.Load<ModConfig>(Constants.ModConfigFileName);
            var defaultConfig = storage.Load<ModConfig>(Constants.ModDefaultConfigFileName);
            if (!manifest.RequireVR)//assume it wasn't present in manifest but present in config, to be removed
            {
                manifest.RequireVR = (config != null && config.RequireVR)
                    || (config == null && defaultConfig != null && defaultConfig.RequireVR);
                storage.Save(manifest, Constants.ModManifestFileName);
            }
            if (config == null && defaultConfig == null)
            {
                config = new ModConfig();
                defaultConfig = new ModConfig();
            }
            else if (defaultConfig != null && config == null)
            {
                config = defaultConfig;
            }
            else if (defaultConfig != null)
            {
                config.MakeConsistentWithDefaults(defaultConfig);
            }
            storage.Save(config, Constants.ModConfigFileName);
            return new ModData(manifest, config, defaultConfig);
        }
    }
}
