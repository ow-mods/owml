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
            var manifestFilenames = Directory.GetFiles(_config.ModsPath, "manifest.json", SearchOption.AllDirectories);
            var mods = new List<IModData>();
            foreach (var manifestFilename in manifestFilenames)
            {
                var json = File.ReadAllText(manifestFilename);
                var manifest = JsonConvert.DeserializeObject<ModManifest>(json);
                manifest.ModFolderPath = manifestFilename.Substring(0, manifestFilename.IndexOf("manifest.json"));
                var modData = GetModData(manifest);
                mods.Add(modData);
            }
            return mods;
        }

        private IModData GetModData(IModManifest manifest)
        {
            var storage = new ModStorage(manifest);
            var config = storage.Load<ModConfig>("config.json");
            var defaultConfig = storage.Load<ModConfig>("default-config.json");
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
                foreach (var setting in defaultConfig.Settings)
                {
                    if (!config.Settings.ContainsKey(setting.Key))
                    {
                        config.Settings.Add(setting.Key, setting.Value);
                    }
                }
            }
            storage.Save(config, "config.json");
            return new ModData(manifest, config, defaultConfig);
        }

    }
}
