﻿using OWML.Common;
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

        private const string EnabledVRDevice = "OpenVR";
        private const int VRRemovedBytes = 2;
        // String that comes right before the bytes we want to patch.
        private const string VRPatchZoneText = "Assets/Scenes/PostCreditScene.unity";
        private const int VRPatchStartZoneOffset = 6;
        private const int BuildSettingsSector = 10;//count from zero

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

        public byte[] GetSectorBytes(byte[] fileBytes, int sector)
        {
            if (sector<0 || sector>=SectorCount)
            {
                return new byte[0];
            }

            var sectorIndex = FirstAddressIndex + sector * AddressStructureSize;
            var sectorStart = BitConverter.ToInt32(fileBytes, sectorIndex) + BlockAddressOffset;
            var sectorEnd = sectorStart + BitConverter.ToInt32(fileBytes, sectorIndex + SizeOffset);

            return fileBytes.Take(sectorEnd).Skip(sectorStart).ToArray();
        }

        public byte[] PatchSectionBytes(byte[] fileBytes, byte[] newBytes, int sectionStart, int sectionOriginalSize, int sector = -1)
        {
            if (sector == -1)
            {
                GetSectorIndex(fileBytes, sectionStart);
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

        public byte[] PatchSectorBytes(byte[] fileBytes, byte[] newBytes, int sector)
        {
            if (sector < 0 || sector >= SectorCount)
            {
                return fileBytes;
            }

            var sectorIndex = FirstAddressIndex + sector * AddressStructureSize;
            var sectorStart = BitConverter.ToInt32(fileBytes, sectorIndex) + BlockAddressOffset;
            var sectorSize = BitConverter.ToInt32(fileBytes, sectorIndex + SizeOffset);
            return PatchSectionBytes(fileBytes, newBytes, sectorStart, sectorSize, sector);
        }

        public void Patch()
        {
            var filePath = $"{_owmlConfig.DataPath}/{FileName}";
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }

            var fileBytes = File.ReadAllBytes(filePath);

            var buildSettingsSectorIndex = FirstAddressIndex + AddressStructureSize * BuildSettingsSector;
            var buildSettingsStartIndex = BitConverter.ToInt32(fileBytes, buildSettingsSectorIndex) + BlockAddressOffset;

            var buildSettingsBytes = GetSectorBytes(fileBytes, BuildSettingsSector);
            var patchStartOffset = FindVRPatchStartOffset(buildSettingsBytes);
            var isAlreadyPatched = FindExistingVRPatch(buildSettingsBytes, patchStartOffset);

            if (isAlreadyPatched)
            {
                _writer.WriteLine("globalgamemanagers already patched.");
                return;
            }

            BackupFile(filePath);
            var patchedBytes = CreateVRPatchFileBytes(fileBytes, buildSettingsStartIndex + patchStartOffset);
            File.WriteAllBytes(filePath, patchedBytes);
            _writer.WriteLine("Successfully patched globalgamemanagers.", MessageType.Success);
        }

        private int GetSectorIndex(byte[] fileBytes, int address)
        {
            int sector = -1;
            for (int sectorIndex = 0; sectorIndex < SectorCount; sectorIndex++)
            {
                var sectorStart = BitConverter.ToInt32(fileBytes, FirstAddressIndex + sectorIndex * AddressStructureSize) + BlockAddressOffset;
                if (sectorStart > address)
                {
                    return sector;
                }
                sector = sectorIndex;
            }
            return sector;
        }

        private int FindVRPatchStartOffset(byte[] sectorBytes)
        {
            var patchZoneBytes = Encoding.ASCII.GetBytes(VRPatchZoneText);
            var patchZoneMatch = 0;
            for (var i = 0; i < sectorBytes.Length; i++)
            {
                var fileByte = sectorBytes[i];
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
                    return i + VRPatchStartZoneOffset;
                }
            }
            throw new Exception("Could not find patch zone in globalgamemanagers. This probably means the VR patch needs to be updated.");
        }

        private bool FindExistingVRPatch(byte[] sectorBytes, int startIndex)
        {
            var existingPatchBytes = Encoding.ASCII.GetBytes(EnabledVRDevice);
            var existingPatchMatch = 0;

            for (var i = startIndex; i < sectorBytes.Length; i++)
            {
                var fileByte = sectorBytes[i];
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

        private byte[] CreateVRPatchFileBytes(byte[] fileBytes, int patchStartIndex)
        {
            // First byte is the number of elements in the array.
            var vrDevicesDeclarationBytes = new byte[] { 1, 0, 0, 0, (byte)EnabledVRDevice.Length, 0, 0, 0 };

            // Bytes that need to be inserted into the file.
            var patchBytes = vrDevicesDeclarationBytes.Concat(Encoding.ASCII.GetBytes(EnabledVRDevice)).ToArray();

            return PatchSectionBytes(fileBytes, patchBytes, patchStartIndex, VRRemovedBytes, BuildSettingsSector);
        }

        private void ShiftInt(byte[] fileBytes, int location, int difference, bool isBigEndian = false)
        {
            int value = isBigEndian ?
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

            // Shift conscutive sector addresses.
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
