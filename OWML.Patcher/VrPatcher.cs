using System;
using System.IO;
using System.Linq;
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

        private static readonly string[] PluginFilenames = { "openvr_api.dll", "OVRPlugin.dll" };
        private static readonly string[] PatchChecksums =
        {
            "cacc71fcb141d936f1b59e57bf10dc52e8edb3481988379f7d95ecb65c4d3c90",
            "d3979abb3b0d2468c3e03e2ee862d5297f5885bd9fc8f3b16cb16805e010d097",
            "7ed2c835ec6653009d29b6b7fa9dc36cd754f64b2f359f7ca635ec6cd4ad8c32",
            "014b0d9b82b20c191ded4d7268975157129ffdb1ca12a30a92a425c783b30e22"
        };

        public VRPatcher(IOwmlConfig owmlConfig, IModConsole writer)
        {
            _owmlConfig = owmlConfig;
            _writer = writer;
            _sha = SHA256.Create();
        }

        public void PatchVR(bool enableVR)
        {
            PatchGlobalManager(enableVR);
            if (enableVR)
            {
                AddPluginFiles();
            }
            else
            {
                RemovePluginFiles();
            }
        }

        private void PatchGlobalManager(bool enableVR)
        {
            var currentPath = _owmlConfig.DataPath + "/globalgamemanagers";
            if (!File.Exists(currentPath))
            {
                _writer.WriteLine("Error: can't find " + currentPath);
                return;
            }

            var currentChecksum = CalculateChecksum(currentPath);
            _writer.WriteLine("Current checksum: " + currentChecksum);

            var backupPath = $"{_owmlConfig.DataPath}/globalgamemanagers.{currentChecksum}.bak";
            if (!File.Exists(backupPath))
            {
                _writer.WriteLine("Taking backup of globalgamemanagers.");
                File.Copy(currentPath, backupPath, true);
            }

            var vrPath = $"{_owmlConfig.DataPath}/globalgamemanagers.{currentChecksum}.vr";
            if (enableVR && !File.Exists(vrPath))
            {
                _writer.WriteLine("Patching globalgamemanagers for VR...");
                if (PatchChecksums.Contains(currentChecksum))
                {
                    var patchPath = $"{_owmlConfig.OWMLPath}VR/{currentChecksum}";
                    ApplyPatch(currentPath, vrPath, patchPath);
                }
                else
                {
                    var patchedChecksum = PatchChecksums.FirstOrDefault(checksum =>
                        CalculateChecksum($"{_owmlConfig.DataPath}/globalgamemanagers.{checksum}.vr") == currentChecksum);
                    if (!string.IsNullOrEmpty(patchedChecksum))
                    {
                        _writer.WriteLine("Already patched! Original checksum: " + patchedChecksum);
                        vrPath = $"{_owmlConfig.DataPath}/globalgamemanagers.{patchedChecksum}.vr";
                    }
                    else
                    {
                        _writer.WriteLine($"Error: invalid checksum: {currentChecksum}. " +
                                          "VR patch for this version of Outer Wilds is not yet supported by OWML.");
                        return;
                    }
                }
            }

            var copyFrom = enableVR ? vrPath : backupPath;
            File.Copy(copyFrom, currentPath, true);
        }

        private string CalculateChecksum(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }
            var bytes = File.ReadAllBytes(filePath);
            var hash = _sha.ComputeHash(bytes);
            var sb = new StringBuilder();
            foreach (var b in hash)
            {
                sb.Append(b.ToString("x2").ToLower());
            }
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
