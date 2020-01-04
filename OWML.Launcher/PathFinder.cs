using System;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using OWML.Common;

namespace OWML.Launcher
{
    public class PathFinder
    {
        private readonly IModConfig _config;
        private readonly IModConsole _writer;

        public PathFinder(IModConfig config, IModConsole writer)
        {
            _config = config;
            _writer = writer;
        }

        public string FindGamePath()
        {
            if (IsValidGamePath(_config.GamePath))
            {
                _writer.WriteLine("Game path is correct in config");
                return _config.GamePath;
            }

            var gamePath = FindInDefaultFolders();
            if (!string.IsNullOrEmpty(gamePath))
            {
                _writer.WriteLine("Game path was found in default paths: " + gamePath);
                return gamePath;
            }

            gamePath = FindInRegistry();
            if (!string.IsNullOrEmpty(gamePath))
            {
                _writer.WriteLine("Game path was found in registry: " + gamePath);
                return gamePath;
            }

            return PromptGamePath();
        }

        private string FindInDefaultFolders()
        {
            var paths = new[]
            {
                AppDomain.CurrentDomain.BaseDirectory + "..",
                "C:/Program Files/Epic Games/OuterWilds",
                "D:/Program Files/Epic Games/OuterWilds",
                "C:/Program Files (x86)/Outer Wilds",
                "D:/Program Files (x86)/Outer Wilds"
            };
            return paths.FirstOrDefault(IsValidGamePath);
        }

        private string FindInRegistry()
        {
            var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Outer Wilds_is1");
            var value = (string)key?.GetValue("InstallLocation");
            return IsValidGamePath(value) ? value : null;
        }

        private string PromptGamePath()
        {
            var gamePath = "";
            while (!IsValidGamePath(gamePath))
            {
                _writer.WriteLine("Game not found at " + gamePath);
                _writer.WriteLine("Please enter the correct game path:");
                gamePath = Console.ReadLine()?.Trim();
            }
            _writer.WriteLine($"Game found at {_config.GamePath}");
            return gamePath;
        }

        private bool IsValidGamePath(string gamePath)
        {
            return !string.IsNullOrEmpty(gamePath) && 
                   Directory.Exists(gamePath) &&
                   Directory.Exists($"{gamePath}/OuterWilds_Data/Managed") &&
                   File.Exists($"{gamePath}/OuterWilds.exe");
        }

    }
}
