using OWML.Common;
using System.IO;

namespace OWML.Patcher
{
    public class VRPatcher
    {
        private readonly IOwmlConfig _owmlConfig;
        private readonly BinaryPatcher _binaryPatcher;
        private readonly VRFilePatcher _vrPatcher;

        private static readonly string[] PluginFilenames = { "openvr_api.dll", "OVRPlugin.dll" };

        public VRPatcher(IOwmlConfig owmlConfig, IModConsole writer)
        {
            _owmlConfig = owmlConfig;
            _binaryPatcher = new BinaryPatcher(_owmlConfig, writer);
            _vrPatcher = new VRFilePatcher(writer, _binaryPatcher);
        }

        public void PatchVR(bool enableVR)
        {
            if (enableVR)
            {
                _vrPatcher.Patch();
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
