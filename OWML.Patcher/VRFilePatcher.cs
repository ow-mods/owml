using System;
using System.Linq;
using System.Text;
using OWML.Common.Enums;
using OWML.Common.Interfaces;

namespace OWML.Patcher
{
    public class VRFilePatcher
    {
        private readonly IModConsole _writer;
        private readonly BinaryPatcher _binaryPatcher;

        private const string EnabledVRDevice = "OpenVR";
        private const int RemovedBytes = 2;
        // String that comes right before the bytes we want to patch.
        private const string PatchZoneText = "Assets/Scenes/PostCreditScene.unity";
        private const int PatchStartZoneOffset = 5;
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
            var patchStartOffset = FindPatchStartOffset(buildSettingsBytes);
            var isAlreadyPatched = FindExistingPatch(buildSettingsBytes, patchStartOffset);

            if (isAlreadyPatched)
            {
                _writer.WriteLine("globalgamemanagers already patched.", MessageType.Message);
                return;
            }

            var patchedBytes = CreatePatchFileBytes(fileBytes, buildSettingsStartIndex + patchStartOffset);
            _binaryPatcher.WriteFileBytes(patchedBytes);
            _writer.WriteLine("Successfully patched globalgamemanagers.", MessageType.Success);
        }

        private int FindPatchStartOffset(byte[] sectorBytes)
        {
            var sectorString = Encoding.ASCII.GetString(sectorBytes);
            var position = sectorString.IndexOf(PatchZoneText);
            if (position < 0)
            {
                throw new Exception("Could not find patch zone in globalgamemanagers. This probably means the VR patch needs to be updated.");
            }
            return position + PatchZoneText.Length + PatchStartZoneOffset;
        }

        private bool FindExistingPatch(byte[] sectorBytes, int startIndex)
        {
            var zoneString = Encoding.ASCII.GetString(sectorBytes.Skip(startIndex).ToArray());
            return zoneString.IndexOf(EnabledVRDevice) >= 0;
        }

        private byte[] CreatePatchFileBytes(byte[] fileBytes, int patchStartIndex)
        {
            var stringsCountBytes = BitConverter.GetBytes(1);
            var stringLengthBytes = BitConverter.GetBytes(EnabledVRDevice.Length);
            var vrDevicesSizeBytes = stringsCountBytes.Concat(stringLengthBytes).ToArray();

            // Bytes that need to be inserted into the file.
            var patchBytes = vrDevicesSizeBytes.Concat(Encoding.ASCII.GetBytes(EnabledVRDevice)).ToArray();

            return _binaryPatcher.PatchSectionBytes(fileBytes, patchBytes, patchStartIndex, RemovedBytes, BuildSettingsSector);
        }
    }
}
