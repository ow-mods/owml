using OWML.Common;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace OWML.Patcher
{
    public class VRFilePatcher
    {
        private readonly IModConsole _writer;
        private readonly BinaryPatcher _binaryPatcher;

        private const string EnabledVRDevice = "OpenVR";
        private const int VRRemovedBytes = 2;
        // String that comes right before the bytes we want to patch.
        private const string VRPatchZoneText = "Assets/Scenes/PostCreditScene.unity";
        private const int VRPatchStartZoneOffset = 6;
        private const int BuildSettingsSector = 10;//count from zero

        public VRFilePatcher(IModConsole writer, BinaryPatcher binaryPatcher)
        {
            _writer = writer;
            _binaryPatcher = binaryPatcher;
        }

        public void Patch()
        {
            var fileBytes = _binaryPatcher.ReadFileBytes();
            var buildSettingsStartIndex = _binaryPatcher.GetSectorInfo(fileBytes, BuildSettingsSector).sectorStart;

            var buildSettingsBytes = _binaryPatcher.GetSectorBytes(fileBytes, BuildSettingsSector);
            var patchStartOffset = FindVRPatchStartOffset(buildSettingsBytes);
            var isAlreadyPatched = FindExistingVRPatch(buildSettingsBytes, patchStartOffset);

            if (isAlreadyPatched)
            {
                _writer.WriteLine("globalgamemanagers already patched.", MessageType.Message);
                return;
            }

            var patchedBytes = CreateVRPatchFileBytes(fileBytes, buildSettingsStartIndex + patchStartOffset);
            _binaryPatcher.WriteFileBytes(patchedBytes);
            _writer.WriteLine("Successfully patched globalgamemanagers.", MessageType.Success);
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

            return _binaryPatcher.PatchSectionBytes(fileBytes, patchBytes, patchStartIndex, VRRemovedBytes, BuildSettingsSector);
        }
    }
}
