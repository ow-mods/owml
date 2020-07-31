using System;
using System.Text;

namespace OWML.Patcher
{
    public class GameVersionReader
    {
        private readonly BinaryPatcher _binaryPatcher;

        private const int PlayerSettingsSector = 0;

        public GameVersionReader(BinaryPatcher binaryPatcher)
        {
            _binaryPatcher = binaryPatcher;
        }

        public string GetGameVersion()
        {
            var sectorBytes = _binaryPatcher.GetSectorBytes(_binaryPatcher.ReadFileBytes(), PlayerSettingsSector);
            for (int i = 4; i < sectorBytes.Length - 2; i++)
            {
                if (sectorBytes[i - 1] == 0 && sectorBytes[i + 1] == '.' && sectorBytes[i + 3] == '.')
                {
                    int length = BitConverter.ToInt32(sectorBytes, i - 4);
                    return Encoding.ASCII.GetString(sectorBytes, i, length);
                }
            }
            return "not found";
        }
    }
}
