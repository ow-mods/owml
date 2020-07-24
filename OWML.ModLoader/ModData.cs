using OWML.Common;
using OWML.ModHelper;

namespace OWML.ModLoader
{
    public class ModData : IModData
    {
        public IModManifest Manifest { get; }
        public IModConfig Config { get; }
        public IModConfig DefaultConfig { get; }
        public bool RequireReload => Config.Enabled != _configSnapshot.Enabled;

        private IModConfig _configSnapshot;

        public ModData(IModManifest manifest, IModConfig config, IModConfig defaultConfig)
        {
            Manifest = manifest;
            Config = config;
            _configSnapshot = new ModConfig() { Enabled = config.Enabled };
            DefaultConfig = defaultConfig;
        }

    }
}
