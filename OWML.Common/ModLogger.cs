using System;
using System.IO;

namespace OWML.Common
{
    public class ModLogger : IModLogger
    {
        private readonly IModConfig _config;

        public ModLogger(IModConfig config)
        {
            _config = config;
        }

        public void Log(string s)
        {
            File.AppendAllText(_config.LogFilePath, $"{DateTime.Now}: {s}{Environment.NewLine}");
        }

    }
}
