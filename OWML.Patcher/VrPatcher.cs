using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using BsDiff;
using OWML.Common;

namespace OWML.Patcher
{
    public class VRPatcher
    {
        private readonly IOwmlConfig _owmlConfig;
        private readonly IModConsole _writer;
        private readonly SHA256 _sha;

        public VRPatcher(IOwmlConfig owmlConfig, IModConsole writer)
        {
            _owmlConfig = owmlConfig;
            _writer = writer;
            _sha = SHA256.Create();
        }

        public void PatchVR(bool enableVR)
        {
            CopyFiles();
            PatchGlobalManager(enableVR);
        }

        private void CopyFiles()
        {
            var filenames = new[] { "openvr_api.dll", "OVRPlugin.dll" };
            foreach (var filename in filenames)
            {
                var from = $"{_owmlConfig.OWMLPath}VR/{filename}";
                var to = $"{_owmlConfig.PluginsPath}/{filename}";
                if (File.Exists(from))
                {
                    File.Copy(from, to, true);
                }
                else
                {
                    _writer.WriteLine("Error: file not found: " + from);
                }
            }
        }

        private void PatchGlobalManager(bool enableVR)
        {
            var originalPath = _owmlConfig.DataPath + "/globalgamemanagers";
            if (!File.Exists(originalPath))
            {
                _writer.WriteLine("Error: can't find " + originalPath);
                return;
            }

            var checksum = CalculateChecksum(originalPath);
            var backupPath = $"{_owmlConfig.DataPath}/globalgamemanagers.{checksum}.bak";
            var vrPath = $"{_owmlConfig.DataPath}/globalgamemanagers.{checksum}.vr";


            if (!File.Exists(backupPath))
            {
                _writer.WriteLine("Taking backup of globalgamemanagers.");
                File.Copy(originalPath, backupPath, true);
            }

            if (enableVR && !File.Exists(vrPath))
            {
                _writer.WriteLine("Patching globalgamemanagers for VR...");
                var patchPath = $"{_owmlConfig.OWMLPath}VR/{checksum}";
                if (File.Exists(patchPath))
                {
                    ApplyPatch(originalPath, vrPath, patchPath);
                }
                else
                {
                    _writer.WriteLine($"Error: invalid checksum: {checksum}. VR patch for this version of Outer Wilds is not yet supported by OWML.");
                    return;
                }
            }

            var copyFrom = enableVR ? vrPath : backupPath;
            File.Copy(copyFrom, originalPath, true);
        }

        private string CalculateChecksum(string filePath)
        {
            var bytes = File.ReadAllBytes(filePath);
            var hash = _sha.ComputeHash(bytes);
            var sb = new StringBuilder();
            foreach (var b in hash)
                sb.Append(b.ToString("x2").ToLower());
            return sb.ToString();
        }

        private void ApplyPatch(string oldFile, string newFile, string patchFile)
        {
            try
            {
                using (var input = new FileStream(oldFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var output = new FileStream(newFile, FileMode.Create))
                {
                    BinaryPatchUtility.Apply(input, () => new FileStream(patchFile, FileMode.Open, FileAccess.Read, FileShare.Read), output);
                }
            }
            catch (Exception ex)
            {
                _writer.WriteLine("Error while patching VR: " + ex);
            }
        }

    }
}
