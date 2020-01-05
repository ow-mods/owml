﻿using System;
using System.IO;
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

    }
}
