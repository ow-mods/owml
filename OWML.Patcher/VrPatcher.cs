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

        public VRPatcher(IOwmlConfig owmlConfig, IModConsole writer)
        {
            _owmlConfig = owmlConfig;
            _writer = writer;
            _sha = SHA256.Create();
        }

        public void PatchVR(bool enableVR)
        {
            if (enableVR)
            {
                PatchGlobalManager();
                AddPluginFiles();
            }
            else
            {
                RemovePluginFiles();
            }
        }

        private void PatchGlobalManager()
        {
            var currentPath = _owmlConfig.DataPath + "/globalgamemanagers";
            if (!File.Exists(currentPath))
            {
                _writer.WriteLine("Error: can't find " + currentPath);
                return;
            }

            // String that comes right before the bytes we want to patch.
            byte[] patchZoneBytes = Encoding.ASCII.GetBytes("Assets/Scenes/PostCreditScene.unity");

            // Bytes that need to be inserted into the file.
            var patchBytes = new byte[] { 1, 0, 0, 0, 6, 0, 0, 0 }.Concat(Encoding.ASCII.GetBytes("OpenVR"));

            var fileBytes = File.ReadAllBytes(currentPath);

            var fileSizeChange = 12;
            // Start position of bytes that define file size.
            var fileSizeStartIndex = 6;

            var originalFileSize = BitConverter.ToInt32(fileBytes, fileSizeStartIndex);
            var patchedFileSizeBytes = BitConverter.GetBytes(originalFileSize + fileSizeChange);

            for (int i = 0; i < patchedFileSizeBytes.Length; i++)
            {
                fileBytes[fileSizeStartIndex + i] = patchedFileSizeBytes[i];
            }

            var currentMatchLength = 0;

            // Indexes of addresses that need to be shifted due to added bytes.
            var addressIndexes = new int[] { 0x2d0, 0x2e0, 0x2f4, 0x308, 0x31c, 0x330, 0x344, 0x358, 0x36c, 0x380 };
            foreach (var index in addressIndexes)
            {
                fileBytes[index] += (byte)fileSizeChange;
            }

            for (var i = 0; i < fileBytes.Length; i++)
            {
                var fileByte = fileBytes[i];
                var matchByte = patchZoneBytes[currentMatchLength];
                if (fileByte == matchByte)
                {
                    currentMatchLength++;
                }
                else
                {
                    currentMatchLength = 0;
                }
                if (currentMatchLength == patchZoneBytes.Length)
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
