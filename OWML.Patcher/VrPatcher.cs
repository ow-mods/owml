using System;
using System.IO;
using BsDiff;
using OWML.Common;

namespace OWML.Patcher
{
    public class VRPatcher
    {
        private readonly IOwmlConfig _owmlConfig;
        private readonly IModConsole _writer;

        public VRPatcher(IOwmlConfig owmlConfig, IModConsole writer)
        {
            _owmlConfig = owmlConfig;
            _writer = writer;
        }

        public void PatchVR(bool enable)
        {
            CopyFiles();
            PatchGlobalManager(enable);
        }

        private void CopyFiles()
        {
            var filenames = new[] { "openvr_api.dll", "OVRPlugin.dll" };
            foreach (var filename in filenames)
            {
                var from = $"{_owmlConfig.OWMLPath}/VR/{filename}";
                var to = $"{_owmlConfig.PluginsPath}/{filename}";
                _writer.WriteLine($"Copying {from} to {to}");
                File.Copy(from, to, true);
            }
        }

        private void PatchGlobalManager(bool enable)
        {
            var originalPath = _owmlConfig.DataPath + "/globalgamemanagers";
            var backupPath = _owmlConfig.DataPath + "/globalgamemanagers.bak";
            var vrPath = _owmlConfig.DataPath + "/globalgamemanagers.vr";
            var patchPath = _owmlConfig.OWMLPath + "VR/patch";

            _writer.WriteLine("original: " + originalPath);
            _writer.WriteLine("backup: " + backupPath);
            _writer.WriteLine("vr: " + vrPath);
            _writer.WriteLine("patch: " + patchPath);
            
            if (!File.Exists(backupPath))
            {
                _writer.WriteLine("Backup...");
                File.Copy(originalPath, backupPath, true);
            }

            if (!File.Exists(vrPath))
            {
                _writer.WriteLine("Patching VR...");
                ApplyPatch(originalPath, vrPath, patchPath);
            }

            var copyFrom = enable ? vrPath : backupPath;
            File.Copy(copyFrom, originalPath, true);
        }

        private void ApplyPatch(string oldFile, string newFile, string patchFile)
        {
            try
            {
                using (FileStream input = new FileStream(oldFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (FileStream output = new FileStream(newFile, FileMode.Create))
                    BinaryPatchUtility.Apply(input, () => new FileStream(patchFile, FileMode.Open, FileAccess.Read, FileShare.Read), output);
            }
            catch (FileNotFoundException ex)
            {
                Console.Error.WriteLine("Could not open '{0}'.", ex.FileName);
            }
        }
    }
}
