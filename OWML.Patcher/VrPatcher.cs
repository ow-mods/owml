using System;
using System.IO;
using BsDiff;
using OWML.Common;

namespace OWML.Patcher
{
    public class VrPatcher
    {
        private readonly IOwmlConfig _owmlConfig;
        private readonly IModConsole _writer;

        public VrPatcher(IOwmlConfig owmlConfig, IModConsole writer)
        {
            _owmlConfig = owmlConfig;
            _writer = writer;
        }

        public void PatchVR()
        {
            CopyFiles();
            PatchGlobalManager();
        }

        private void CopyFiles()
        {
            var filenames = new[] { "openvr_api.dll", "OVRPlugin.dll" };
            foreach (var filename in filenames)
            {
                var from = _owmlConfig.OWMLPath + filename;
                var to = $"{_owmlConfig.PluginsPath}/{filename}";
                File.Copy(from, to, true);
            }
        }

        private void PatchGlobalManager()
        {
            var oldFilename = _owmlConfig.ManagedPath + "globalgamemanagers";
            var backupFilename = _owmlConfig.ManagedPath + "globalgamemanagers.bak";
            var newFilename = _owmlConfig.ManagedPath + "globalgamemanagers.new";
            var patchFilename = _owmlConfig.OWMLPath + "vr-patch"; // todo

            File.Copy(oldFilename, backupFilename, true);

            try
            {
                using (var input = new FileStream(oldFilename, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var output = new FileStream(newFilename, FileMode.Create))
                {
                    BinaryPatchUtility.Apply(input, () => new FileStream(patchFilename, FileMode.Open, FileAccess.Read, FileShare.Read), output);
                }
            }
            catch (Exception ex)
            {
                _writer.WriteLine("Error while patching globalgamemanagers: " + ex);
            }

            File.Copy(newFilename, oldFilename, true);
        }

    }
}
