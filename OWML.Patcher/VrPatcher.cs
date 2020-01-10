using System;
using System.Diagnostics;
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
            var original = _owmlConfig.DataPath + "/globalgamemanagers";
            var backup = _owmlConfig.DataPath + "/globalgamemanagers.bak";
            var vr = _owmlConfig.DataPath + "/globalgamemanagers.vr";
            var patch = _owmlConfig.OWMLPath + "VR/patch";

            _writer.WriteLine("original: " + original);
            _writer.WriteLine("backup: " + backup);
            _writer.WriteLine("vr: " + vr);
            _writer.WriteLine("patch: " + patch);
            
            if (!File.Exists(backup))
            {
                _writer.WriteLine("Backup...");
                File.Copy(original, backup, true);
            }

            if (!File.Exists(vr))
            {
                _writer.WriteLine("Patching VR...");
                Process.Start(null, $"{backup} {vr} {patch}");
            }

            var copyFrom = enable ? vr : backup;
            File.Copy(copyFrom, original, true);
        }

    }
}
