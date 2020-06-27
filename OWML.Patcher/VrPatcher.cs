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
        // Indexes of addresses that need to be shifted due to added bytes.
        private static readonly int[] addressIndexes = { 0x2d0, 0x2e0, 0x2f4, 0x308, 0x31c, 0x330, 0x344, 0x358, 0x36c, 0x380 };

        private const string EnabledVRDevice = "OpenVR";
        private const int RemovedBytes = 2;
        // String that comes right before the bytes we want to patch.
        private const string PatchZoneText = "Assets/Scenes/PostCreditScene.unity";
        private const int FileSizeStartIndex = 4;
        private const int FileSizeEndIndex = FileSizeStartIndex + 4;

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

            byte[] patchZoneBytes = Encoding.ASCII.GetBytes(PatchZoneText);
            byte[] existingPatchBytes = Encoding.ASCII.GetBytes(EnabledVRDevice);

            var patchZoneMatch = 0;
            var existingPatchMatch = 0;
            var patchStartIndex = -1;
            var patchStartIndexOffset = 6;
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
                        patchStartIndex = i + patchStartIndexOffset;
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
                // First byte is the number of elements in the array.
                var vrDevicesDeclarationBytes = new byte[] { 1, 0, 0, 0, (byte)EnabledVRDevice.Length, 0, 0, 0 };

                // Bytes that need to be inserted into the file.
                var patchBytes = vrDevicesDeclarationBytes.Concat(Encoding.ASCII.GetBytes(EnabledVRDevice));

                PatchFileSize(fileBytes, patchBytes.Count());

                // Split the file in two parts. The patch bytes will be inserted between these parts.
                var originalFirstPart = fileBytes.Take(patchStartIndex);
                var originalSecondPart = fileBytes.Skip(patchStartIndex + RemovedBytes);

                var patchedBytes = originalFirstPart
                    .Concat(patchBytes)
                    .Concat(originalSecondPart)
                    .ToArray();

                File.WriteAllBytes(currentPath + ".patched-rai", patchedBytes);
            }
        }

        private void PatchFileSize(byte[] fileBytes, int patchSize)
        {
            // Read file size from original file. Reversed due to big endianness.
            var originalFileSizeBytes = fileBytes.Take(FileSizeEndIndex).Skip(FileSizeStartIndex).Reverse().ToArray();
            var originalFileSize = BitConverter.ToInt32(originalFileSizeBytes, 0);

            // Generate bytes for new patched file.
            var fileSizeChange = patchSize - RemovedBytes;
            var patchedFileSize = originalFileSize + fileSizeChange;
            var patchedFileSizeBytes = BitConverter.GetBytes(patchedFileSize).Reverse().ToArray();

            // Overwrite original file size bytes with patched size.
            for (int i = 0; i < patchedFileSizeBytes.Length; i++)
            {
                fileBytes[FileSizeStartIndex + i] = patchedFileSizeBytes[i];
            }

            // Shift addresses where necessary.
            foreach (var index in addressIndexes)
            {
                fileBytes[index] += (byte)fileSizeChange;
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
