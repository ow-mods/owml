using OWML.Common;
using System;
using System.Linq;
using System.Text;

namespace OWML.Patcher
{
    public class GameVersionReader
    {
        private readonly IModConsole _writer;
        private readonly BinaryPatcher _binaryPatcher;

        private const int PlayerSettingsSector = 0;

        public GameVersionReader(IModConsole writer, BinaryPatcher binaryPatcher)
        {
            _writer = writer;
            _binaryPatcher = binaryPatcher;
        }

        public string GetGameVersion()
        {
            var sectorBytes = _binaryPatcher.GetSectorBytes(_binaryPatcher.ReadFileBytes(), PlayerSettingsSector);
            for (int i = 4; i < sectorBytes.Length - 2; i++)
            {
                if (sectorBytes[i - 4] == 0 && sectorBytes[i - 3] == 0 && sectorBytes[i - 2] == 0 &&
                    sectorBytes[i] == '.' && sectorBytes[i + 2] == '.')
                {
                    int length = BitConverter.ToInt32(sectorBytes, i - 5);
                    return Encoding.ASCII.GetString(sectorBytes, i - 1, length);
                }
            }
            return "not found";
        }
    }
}
