using OWML.Common;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace OWML.Patcher
{
    public class BinaryPatcher
    {
        private readonly IOwmlConfig _owmlConfig;
        private readonly IModConsole _writer;

        // Indexes of addresses that need to be shifted due to added bytes.
        private static readonly int[] _addressIndexes = { 0x2d0, 0x2e0, 0x2f4, 0x308, 0x31c, 0x330, 0x344, 0x358, 0x36c, 0x380 };

        private const string EnabledVRDevice = "OpenVR";
        private const int RemovedBytes = 2;
        // String that comes right before the bytes we want to patch.
        private const string PatchZoneText = "Assets/Scenes/PostCreditScene.unity";
        private const int PatchStartZoneOffset = 6;
        private const int FileSizeStartIndex = 4;
        private const int FileSizeEndIndex = FileSizeStartIndex + 4;
        private const string FileName = "globalgamemanagers";
        private const string BackupSuffix = ".bak";
        private const int BuildSettingsStartAddressIndex = 0x2CC;
        private const int BuildSettingsSizeIndex = 0x2D0;
        private const int BlockAddressOffset = 0x1000;

        public BinaryPatcher(IOwmlConfig owmlConfig, IModConsole writer)
        {
            _owmlConfig = owmlConfig;
            _writer = writer;
        }

        public void Patch()
        {
            var filePath = $"{_owmlConfig.DataPath}/{FileName}";
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }

            var fileBytes = File.ReadAllBytes(filePath);

            var buildSettingsStartIndex = BitConverter.ToInt32(fileBytes, BuildSettingsStartAddressIndex) + BlockAddressOffset;
            var buildSettingsSize = BitConverter.ToInt32(fileBytes, BuildSettingsSizeIndex);
            var buildSettingsEndIndex = buildSettingsStartIndex + buildSettingsSize;

            var patchStartIndex = FindPatchStartIndex(fileBytes, buildSettingsStartIndex, buildSettingsEndIndex);
            var isAlreadyPatched = FindExistingPatch(fileBytes, patchStartIndex, buildSettingsEndIndex);

            if (isAlreadyPatched)
            {
                _writer.WriteLine("globalgamemanagers already patched.");
                return;
            }

            BackupFile(filePath);
            var patchedBytes = CreatePatchedFileBytes(fileBytes, patchStartIndex);
            File.WriteAllBytes(filePath, patchedBytes);
            _writer.WriteLine("Successfully patched globalgamemanagers.");
        }

        private int FindPatchStartIndex(byte[] fileBytes, int startIndex, int endIndex)
        {
            byte[] patchZoneBytes = Encoding.ASCII.GetBytes(PatchZoneText);
            var patchZoneMatch = 0;
            for (var i = startIndex; i < endIndex; i++)
            {
                var fileByte = fileBytes[i];
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
                    return i + PatchStartZoneOffset;
                }
            }
            throw new Exception("Could not find patch zone in globalgamemanagers. This probably means the VR patch needs to be updated.");
        }

        private bool FindExistingPatch(byte[] fileBytes, int startIndex, int endIndex)
        {
            byte[] existingPatchBytes = Encoding.ASCII.GetBytes(EnabledVRDevice);
            var existingPatchMatch = 0;

            for (var i = startIndex; i < endIndex; i++)
            {
                var fileByte = fileBytes[i];
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
                    return true;
                }
            }
            return false;
        }

        private byte[] CreatePatchedFileBytes(byte[] fileBytes, int patchStartIndex)
        {
            // First byte is the number of elements in the array.
            var vrDevicesDeclarationBytes = new byte[] { 1, 0, 0, 0, (byte)EnabledVRDevice.Length, 0, 0, 0 };

            // Bytes that need to be inserted into the file.
            var patchBytes = vrDevicesDeclarationBytes.Concat(Encoding.ASCII.GetBytes(EnabledVRDevice));

            PatchFileSize(fileBytes, patchBytes.Count());

            // Split the file in two parts. The patch bytes will be inserted between these parts.
            var originalFirstPart = fileBytes.Take(patchStartIndex);
            var originalSecondPart = fileBytes.Skip(patchStartIndex + RemovedBytes);

            return originalFirstPart
                .Concat(patchBytes)
                .Concat(originalSecondPart)
                .ToArray();
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
            foreach (var startIndex in _addressIndexes)
            {
                var address = BitConverter.ToInt32(fileBytes, startIndex);
                var patchedAddressBytes = BitConverter.GetBytes(address + fileSizeChange);
                for (int i = 0; i < patchedAddressBytes.Length; i++)
                {
                    fileBytes[startIndex + i] = patchedAddressBytes[i];
                }
            }
        }

        private void BackupFile(string path)
        {
            File.Copy(path, path + BackupSuffix, true);
        }

        public void RestoreFromBackup()
        {
            var filePath = $"{_owmlConfig.DataPath}/{FileName}";
            var backupPath = filePath + BackupSuffix;
            if (File.Exists(backupPath))
            {
                File.Copy(backupPath, filePath, true);
                File.Delete(backupPath);
            }
        }
    }
}
