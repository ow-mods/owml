using System;
using System.IO;
using System.Linq;
using System.Text;
using OWML.Common;

namespace OWML.Patcher
{
    public class VRPatcher
    {
        private readonly IOwmlConfig _owmlConfig;
        private readonly IModConsole _writer;

        private static readonly string[] PluginFilenames = { "openvr_api.dll", "OVRPlugin.dll" };

        public VRPatcher(IOwmlConfig owmlConfig, IModConsole writer)
        {
            _owmlConfig = owmlConfig;
            _writer = writer;
        }

        public void PatchVR(bool enableVR)
        {
            PatchGlobalManager();
            if (enableVR)
            {
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

            // Consider file already patched if this string is present.
            byte[] existingPatchBytes = Encoding.ASCII.GetBytes("OpenVR");

            var patchZoneMatch = 0;
            var existingPatchMatch = 0;
            var patchStartIndex = -1;
            var isAlreadyPatched = false;

            var fileBytes = File.ReadAllBytes(currentPath);
            for (var i = 0; i < fileBytes.Length; i++)
            {
                var fileByte = fileBytes[i];
                if (patchStartIndex == -1)
                {
                    var patchZoneByte = patchZoneBytes[patchZoneMatch];
                    if (fileByte == patchZoneByte)
                    {
                        patchZoneMatch++;
                    }
                    else
                    {
                        patchZoneMatch = 0;
                    }
                    if (patchZoneMatch == patchZoneBytes.Length)
                    {
                        _writer.WriteLine("Found match ending in index", i);

                        patchStartIndex = i + 6;
                    }
                }
                else
                {
                    var existingPatchByte = existingPatchBytes[existingPatchMatch];
                    if (fileByte == existingPatchByte)
                    {
                        existingPatchMatch++;
                    }
                    else
                    {
                        existingPatchMatch = 0;
                    }
                    if (existingPatchMatch == existingPatchBytes.Length)
                    {
                        _writer.WriteLine("globalgamemanagers already patched");
                        isAlreadyPatched = true;

                        break;
                    }
                }
            }

            if (patchStartIndex != -1 && !isAlreadyPatched)
            {
                var fileSizeChange = 12;
                // Start position of bytes that define file size.
                var fileSizeStartIndex = 4;

                var originalFileSizeBytes = fileBytes.Take(fileSizeStartIndex + 4).Skip(fileSizeStartIndex).Reverse().ToArray();
                var originalFileSize = BitConverter.ToInt32(originalFileSizeBytes, 0);
                var patchedFileSizeBytes = BitConverter.GetBytes(originalFileSize + fileSizeChange).Reverse().ToArray();

                // TODO fix this
                for (int i = 0; i < patchedFileSizeBytes.Length; i++)
                {
                    fileBytes[fileSizeStartIndex + i] = patchedFileSizeBytes[i];
                }

                // Indexes of addresses that need to be shifted due to added bytes.
                var addressIndexes = new int[] { 0x2d0, 0x2e0, 0x2f4, 0x308, 0x31c, 0x330, 0x344, 0x358, 0x36c, 0x380 };
                foreach (var index in addressIndexes)
                {
                    fileBytes[index] += (byte)fileSizeChange;
                }

                var originalFirstPart = fileBytes.Take(patchStartIndex);
                var originalSecondPart = fileBytes.Skip(patchStartIndex + 2);

                // Bytes that need to be inserted into the file.
                var patchBytes = new byte[] { 1, 0, 0, 0, 6, 0, 0, 0 }.Concat(Encoding.ASCII.GetBytes("OpenVR"));

                var patchedBytes = originalFirstPart
                    .Concat(patchBytes)
                    .Concat(originalSecondPart)
                    .ToArray();

                File.WriteAllBytes(currentPath + ".patched-rai", patchedBytes);
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
