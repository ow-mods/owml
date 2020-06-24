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

            var match = "Assets/Scenes/PostCreditScene.unity";
            byte[] matchBytes = Encoding.ASCII.GetBytes(match);

            var patchFirstPart = new byte[] { 1, 0, 0, 0, 6, 0, 0, 0 };
            var patchSecondPart = Encoding.ASCII.GetBytes("OpenVR");
            var patchBytes = patchFirstPart.Concat(patchSecondPart);

            var incIndexes = new int[] { 0x7, 0x2d0, 0x2e0, 0x2f4, 0x308, 0x31c, 0x330, 0x344, 0x358, 0x36c, 0x380 };

            var fileBytes = File.ReadAllBytes(currentPath);

            _writer.WriteLine("fileBytes length", fileBytes.Length);

            var currentMatchLength = 0;
            for (var i = 0; i < fileBytes.Length; i++)
            {
                if (incIndexes.Contains(i))
                {
                    fileBytes[i] += 12;
                }

                var fileByte = fileBytes[i];
                var matchByte = matchBytes[currentMatchLength];
                if (fileByte == matchByte)
                {
                    currentMatchLength++;
                }
                else
                {
                    currentMatchLength = 0;
                }
                if (currentMatchLength == matchBytes.Length)
                {
                    _writer.WriteLine("Found match ending in index", i);

                    var startIndex = i + 6;

                    var originalFirstPart = fileBytes.Take(startIndex);
                    var originalSecondPart = fileBytes.Skip(startIndex + 2);

                    var patchedBytes = originalFirstPart
                        .Concat(patchBytes)
                        .Concat(originalSecondPart)
                        .ToArray();

                    File.WriteAllBytes(currentPath + ".patched-rai", patchedBytes);

                    break;
                }
            }
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
