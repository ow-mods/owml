using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using OWML.Common;
using OWML.ModHelper;

namespace OWML.ModLoader
{
    public class ModData : IModData
    {
        public IModManifest Manifest { get; }
        public IModConfig Config { get; private set; }
        public IModConfig UserConfig { get; private set; }
        public bool RequireReload => Config.Enabled != _configSnapshot.Enabled;
        public bool RequireVR => Manifest.RequireVR;

        public bool Enabled => Config.Enabled;


        private IModConfig _configSnapshot;

        public ModData(IModManifest manifest, IModConfig config, IModConfig defaultConfig)
        {
            Manifest = manifest;
            Config = new ModMergedConfig(config, defaultConfig, manifest);
            UserConfig = config;
            UpdateSnapshot();
        }

        public void UpdateSnapshot()
        {
            _configSnapshot = Config.Copy();
        }

        public void ResetConfigToDefaults()
        {
            Config.Settings.Clear();
        }
    }
}
