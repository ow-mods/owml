using OWML.Common;
using OWML.ModHelper;

namespace OWML.ModLoader
{
    public class ModData : IModData
    {
        public IModManifest Manifest { get; }
        public IModConfig Config { get; }

        public ModData(IModManifest manifest, IModConfig config)
        {
            Manifest = manifest;
            Config = config;
        }

    }
}
