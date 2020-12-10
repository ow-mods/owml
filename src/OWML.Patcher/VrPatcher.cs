using System.IO;
using OWML.Common;

namespace OWML.Patcher
{
    public class VRPatcher : IVRPatcher
    {
        private readonly IOwmlConfig _owmlConfig;
        private readonly IBinaryPatcher _binaryPatcher;
        private readonly IVRFilePatcher _vrFilePatcher;

        private static readonly string[] PluginFilenames =
        {
            "openvr_api.dll",
            "OVRPlugin.dll"
        };

        public VRPatcher(IOwmlConfig owmlConfig, IBinaryPatcher binaryPatcher, IVRFilePatcher vrFilePatcher)
        {
            _owmlConfig = owmlConfig;
            _binaryPatcher = binaryPatcher;
            _vrFilePatcher = vrFilePatcher;
        }

        public void PatchVR(bool enableVR)
        {
            if (enableVR)
            {
                _vrFilePatcher.Patch();
                AddPluginFiles();
            }
            else
            {
                _binaryPatcher.RestoreFromBackup();
                RemovePluginFiles();
            }
        }

        private void AddPluginFiles()
        {
            foreach (var filename in PluginFilenames)
            {
                var from = $"{_owmlConfig.OWMLPath}VR/{filename}";
                var to = $"{_owmlConfig.PluginsPath}/{filename}";
                File.Copy(from, to, true);
            }
        }

        private void RemovePluginFiles()
        {
            foreach (var filename in PluginFilenames)
            {
                var path = $"{_owmlConfig.PluginsPath}/{filename}";
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

    }
}
