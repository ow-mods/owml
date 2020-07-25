using OWML.Common;

namespace OWML.ModLoader
{
    public class ModData : IModData
    {
        public IModManifest Manifest { get; }
        public IModConfig Config { get; }
        public IModConfig DefaultConfig { get; }
        public bool RequireVR => Manifest.RequireVR
            || (Config != null && Config.RequireVR)
            || (Config == null && DefaultConfig != null && DefaultConfig.RequireVR);

    public ModData(IModManifest manifest, IModConfig config, IModConfig defaultConfig)
        {
            Manifest = manifest;
            Config = config;
            DefaultConfig = defaultConfig;
        }

    }
}
