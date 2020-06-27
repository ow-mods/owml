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
        private readonly string _filePath;

        private const string EnabledVRDevice = "OpenVR";
        private const int RemovedBytes = 2;
        // String that comes right before the bytes we want to patch.
        private const string PatchZoneText = "Assets/Scenes/PostCreditScene.unity";
        private const int PatchStartZoneOffset = 6;
        private const int FileSizeStartIndex = 4;
        private const int FileSizeEndIndex = FileSizeStartIndex + 4;
        private const string FileName = "globalgamemanagers";
        private const string BackupSuffix = ".bak";

        public BinaryPatcher(IOwmlConfig owmlConfig, IModConsole writer)
        {
            _owmlConfig = owmlConfig;
            _writer = writer;
            _filePath = $"{_owmlConfig.DataPath}/{FileName}";
        }

        public void Patch()
        {
            if (!File.Exists(_filePath))
            {
                _writer.WriteLine("Error: can't find " + _filePath);
                return;
            }

            byte[] patchZoneBytes = Encoding.ASCII.GetBytes(PatchZoneText);
            byte[] existingPatchBytes = Encoding.ASCII.GetBytes(EnabledVRDevice);

            var patchZoneMatch = 0;
            var existingPatchMatch = 0;
            var patchStartIndex = -1;
            var isAlreadyPatched = false;

            var fileBytes = File.ReadAllBytes(_filePath);
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
                        patchStartIndex = i + PatchStartZoneOffset;
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
                        isAlreadyPatched = true;
                        break;
                    }
                }
            }

            if (isAlreadyPatched)
            {
                _writer.WriteLine("globalgamemanagers already patched.");
                return;
            }

            if (patchStartIndex == -1)
            {
                _writer.WriteLine("Error: could not find patch zone in globalgamemanagers. This probably means the VR patch needs to be updated.");
            }
            else
            {
                BackupFile(_filePath);
                var patchedBytes = CreatePatchedFileBytes(fileBytes, patchStartIndex);
                File.WriteAllBytes(_filePath, patchedBytes);
                _writer.WriteLine("Successfully patched globalgamemanagers.");
            }
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
            foreach (var index in _addressIndexes)
            {
                fileBytes[index] += (byte)fileSizeChange;
            }
        }

        private void BackupFile(string path)
        {
            File.Copy(path, path + BackupSuffix, true);
        }

        public void RestoreFromBackup()
        {
            var backupPath = _filePath + BackupSuffix;
            if (File.Exists(backupPath))
            {
                File.Copy(backupPath, _filePath, true);
                File.Delete(backupPath);
            }
        }
    }
}
