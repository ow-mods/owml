using System.IO;
using System.Text.RegularExpressions;
using OWML.Common;

namespace OWML.Launcher
{
    public class GameVersion
    {
        private const string VersionPattern = @"\d+\.\d+\.\d+\.\d+";

        private readonly IOwmlConfig _config;

        public GameVersion(IOwmlConfig config)
        {
            _config = config;
        }

        public string GetVersion()
        {
            var globalgamemanagers = File.ReadAllText($"{_config.DataPath}/globalgamemanagers");
            return new Regex(VersionPattern).Match(globalgamemanagers).Value;
        }
    }
}
