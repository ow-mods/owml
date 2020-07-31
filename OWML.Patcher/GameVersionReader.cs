using System;
using System.Text;
using System.Text.RegularExpressions;

namespace OWML.Patcher
{
    public class GameVersionReader
    {
        private readonly BinaryPatcher _binaryPatcher;

        private const int PlayerSettingsSector = 0;
        private const string VersionPattern = "[0-9]{1,2}.[0-9]{1,2}.[0-9]{1,4}[.]{0,1}[0-9]{0,4}";

        public GameVersionReader(BinaryPatcher binaryPatcher)
        {
            _binaryPatcher = binaryPatcher;
        }

        public string GetGameVersion()
        {
            var sectorBytes = _binaryPatcher.GetSectorBytes(_binaryPatcher.ReadFileBytes(), PlayerSettingsSector);
            var sectorString = Encoding.ASCII.GetString(sectorBytes);
            var match = Regex.Match(sectorString, VersionPattern);
            return match.Success? match.Value: "not found";
        }
    }
}
