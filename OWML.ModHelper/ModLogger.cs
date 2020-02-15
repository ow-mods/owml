using System;
using System.IO;
using System.Linq;
using OWML.Common;

namespace OWML.ModHelper
{
    public class ModLogger : IModLogger
    {
        public static ModLogger Instance { get; private set; }

        private readonly IOwmlConfig _config;

        public ModLogger(IOwmlConfig config)
        {
            Instance = this;
            _config = config;
        }

        public void Log(string s)
        {
            File.AppendAllText(_config.LogFilePath, $"{DateTime.Now}: {s}{Environment.NewLine}");
        }

        public void Log(params object[] objects)
        {
            Log(string.Join(" ", objects.Select(o => o.ToString()).ToArray()));
        }

    }
}
