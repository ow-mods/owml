using OWML.Common;
using OWML.ModHelper;
using System.Collections.Generic;

namespace OWML.ModLoader
{
    public class ModData : IModData
    {
        public IModManifest Manifest { get; }
        public IModConfig Config { get; private set; }
        public IModConfig DefaultConfig { get; }

        public ModData(IModManifest manifest, IModConfig config, IModConfig defaultConfig)
        {
            Manifest = manifest;
            Config = config;
            DefaultConfig = defaultConfig;
        }

        public void ResetConfig()
        {
            Config = new ModConfig
            {
                Enabled = DefaultConfig.Enabled,
                RequireVR = DefaultConfig.RequireVR,
                Settings = new Dictionary<string, object>(DefaultConfig.Settings)
            };
        }

    }
}
