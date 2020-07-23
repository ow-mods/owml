using System;
using System.IO;
using System.Linq;
using OWML.Common;

namespace OWML.ModHelper
{
    public class ModLogger : IModLogger
    {
        [Obsolete("Use ModHelper.Logger instead")]
        public static ModLogger Instance { get; private set; }

        public static event Action<IModManifest, string> OnLog;


        private static IOwmlConfig _config;
        private readonly IModManifest _manifest;

        public ModLogger(IOwmlConfig config, IModManifest manifest)
        {
            if (manifest.Name == "OWML")
            {
                Instance = this;
            }
            if (_config == null)
            {
                _config = config;
            }
            _manifest = manifest;
        }

        public void Log(string s)
        {
            OnLog?.Invoke(_manifest, s);
            var message = $"[{_manifest.Name}]: {s}";
            LogInternal(message);
        }

        public void Log(params object[] objects)
        {
            Log(string.Join(" ", objects.Select(o => o.ToString()).ToArray()));
        }

        private static void LogInternal(string message)
        {
            File.AppendAllText(_config.LogFilePath, $"{DateTime.Now}: {message}{Environment.NewLine}");
        }

    }
}
