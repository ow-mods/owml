using System;
using System.IO;
using System.Linq;
using OWML.Common;

namespace OWML.Patcher
{
    public class BinaryPatcher : IBinaryPatcher
    {
        private readonly IOwmlConfig _owmlConfig;
        private readonly IModConsole _writer;

        private const int FileSizeStartIndex = 4;
        private const string FileName = "globalgamemanagers";
        private const string BackupSuffix = ".bak";
        private const int BlockAddressOffset = 0x1000;
        private const int FirstAddressIndex = 0x204;
        private const int AddressStructureSize = 0x14;
        private const int SizeOffset = 0x4;
        private const int SectorCount = 20;

        public BinaryPatcher(IOwmlConfig owmlConfig, IModConsole writer)
        {
            _owmlConfig = owmlConfig;
            _writer = writer;
        }

        public ISectorInfo GetSectorInfo(byte[] fileBytes, int sector)
        {
            if (sector < 0 || sector >= SectorCount)
            {
                return new SectorInfo(-1, -1);
            }
            var sectorIndex = FirstAddressIndex + sector * AddressStructureSize;
            var sectorStart = BitConverter.ToInt32(fileBytes, sectorIndex) + BlockAddressOffset;
            var sectorSize = BitConverter.ToInt32(fileBytes, sectorIndex + SizeOffset);
            return new SectorInfo(sectorStart, sectorSize);
        }

        public byte[] ReadFileBytes()
        {
            var filePath = $"{_owmlConfig.DataPath}/{FileName}";
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }

            return File.ReadAllBytes(filePath);
        }

        public void WriteFileBytes(byte[] fileBytes)
        {
            var filePath = $"{_owmlConfig.DataPath}/{FileName}";
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }

            BackupFile(filePath);
            File.WriteAllBytes(filePath, fileBytes);
        }

        public byte[] GetSectorBytes(byte[] fileBytes, int sector)
        {
            if (sector < 0 || sector >= SectorCount)
            {
                return new byte[0];
            }

            var sectorInfo = GetSectorInfo(fileBytes, sector);
            return fileBytes.Take(sectorInfo.SectorEnd).Skip(sectorInfo.SectorStart).ToArray();
        }

        public byte[] PatchSectionBytes(byte[] fileBytes, byte[] newBytes, int sectionStart, int sectionOriginalSize, int sector = -1)
        {
            if (sector == -1)
            {
                sector = GetSectorIndex(fileBytes, sectionStart);
            }
            if (sector < 0 || sector >= SectorCount)
            {
                if (sector == -1)
                {
                    _writer.WriteLine("Error - Patching header is not supported", MessageType.Error);
                }
                return fileBytes;
            }

            var sectionEnd = sectionStart + sectionOriginalSize;

            var sizeDifference = newBytes.Length - sectionOriginalSize;

            var newFileBytes = fileBytes.Take(sectionStart).Concat(newBytes).Concat(fileBytes.Skip(sectionEnd)).ToArray();

            PatchFileSize(newFileBytes, sizeDifference, sector);

            return newFileBytes;
        }

        private int GetSectorIndex(byte[] fileBytes, int address)
        {
            var sector = -1;
            for (var sectorIndex = 0; sectorIndex < SectorCount; sectorIndex++)
            {
                var sectorStart = GetSectorInfo(fileBytes, sectorIndex).SectorStart;
                if (sectorStart > address)
                {
                    return sector;
                }
                sector = sectorIndex;
            }
            return sector;
        }

        private void ShiftInt(byte[] fileBytes, int location, int difference, bool isBigEndian = false)
        {
            var value = isBigEndian ?
                BitConverter.ToInt32(fileBytes.Take(location + 4).Reverse().ToArray(), 0) :
                BitConverter.ToInt32(fileBytes, location);

            var increasedValueBytes = BitConverter.GetBytes(value + difference);
            if (isBigEndian)
            {
                increasedValueBytes = increasedValueBytes.Reverse().ToArray();
            }

            increasedValueBytes.CopyTo(fileBytes, location);
        }

        private void PatchFileSize(byte[] fileBytes, int patchSize, int patchedSector)
        {
            // Shift file size.
            ShiftInt(fileBytes, FileSizeStartIndex, patchSize, true);

            // Shift sector size.
            var sizeIndex = FirstAddressIndex + patchedSector * AddressStructureSize + SizeOffset;
            ShiftInt(fileBytes, sizeIndex, patchSize);

            // Shift consecutive sector addresses.
            for (var sector = patchedSector + 1; sector < SectorCount; sector++)
            {
                var startIndex = FirstAddressIndex + sector * AddressStructureSize;
                ShiftInt(fileBytes, startIndex, patchSize);
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
