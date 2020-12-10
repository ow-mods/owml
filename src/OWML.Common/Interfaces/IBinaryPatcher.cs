namespace OWML.Common
{
    public interface IBinaryPatcher
    {
        void RestoreFromBackup();

        byte[] ReadFileBytes();

        ISectorInfo GetSectorInfo(byte[] bytes, int sector);

        byte[] GetSectorBytes(byte[] bytes, int sector);

        void WriteFileBytes(byte[] bytes);

        byte[] PatchSectionBytes(byte[] fileBytes, byte[] patchBytes, int patchStartIndex, int removedBytes, int buildSettingsSector);
    }
}
