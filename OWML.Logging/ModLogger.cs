using System;
using System.IO;
using System.Linq;
using OWML.Common;

namespace OWML.Logging
{
    public class ModLogger : IModLogger
    {
        private static IOwmlConfig _config;
        private readonly IModManifest _manifest;
        private static string _logFileName;

        public ModLogger(IOwmlConfig config, IModManifest manifest, string logFileName)
        {
            if (_config == null)
            {
                _config = config;
            }
            _manifest = manifest;
            _logFileName = logFileName;
        }

        public void Log(string s)
        {
            var message = $"[{_manifest.Name}]: {s}";
            LogInternal(message);
        }

        public void Log(params object[] objects)
        {
            Log(string.Join(" ", objects.Select(o => o.ToString()).ToArray()));
        }

        private static void LogInternal(string message)
        {
            File.AppendAllText(_logFileName, $"{DateTime.Now}: {message}{Environment.NewLine}");
        }
    }
}
