using System;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using Newtonsoft.Json;
using OWML.Common;

namespace OWML.Launcher
{
    public class PathFinder
    {
        private readonly IOwmlConfig _config;
        private readonly IModConsole _writer;

        public PathFinder(IOwmlConfig config, IModConsole writer)
        {
            _config = config;
            _writer = writer;
        }

        public string FindGamePath()
        {
            return ValidateCurrentPath() ??
                   FindGameInDefaultFolders() ??
                   FindGameInEGS() ??
                   FindGameInSteam() ??
                   PromptGamePath();
        }

        private string ValidateCurrentPath()
        {
            if (IsValidGamePath(_config.GamePath))
            {
                return _config.GamePath;
            }
            _writer.WriteLine($"Current game path is not valid: {_config.GamePath}");
            return null;
        }

        private string FindGameInDefaultFolders()
        {
            var paths = new[]
            {
                $"{AppDomain.CurrentDomain.BaseDirectory}..",
                "C:/Program Files (x86)/Outer Wilds",
                "D:/Program Files (x86)/Outer Wilds",
                "E:/Program Files (x86)/Outer Wilds",
                "F:/Program Files (x86)/Outer Wilds",
                "C:/Games/Outer Wilds",
                "D:/Games/Outer Wilds",
                "E:/Games/Outer Wilds",
                "F:/Games/Outer Wilds"
            };
            var gamePath = paths.FirstOrDefault(IsValidGamePath);
            if (!string.IsNullOrEmpty(gamePath))
            {
                return gamePath;
            }
            _writer.WriteLine("Game not found in default locations.");
            return null;
        }

        private string FindGameInEGS()
        {
            var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Epic Games\EpicGamesLauncher");
            var appDataPath = (string)key?.GetValue("AppDataPath");
            if (string.IsNullOrEmpty(appDataPath))
            {
                _writer.WriteLine("EGS not found in Registry.");
                return null;
            }
            var manifestsPath = $"{appDataPath}Manifests";
            if (!Directory.Exists(manifestsPath))
            {
                _writer.WriteLine($"EGS manifests folder not found: {manifestsPath}");
                return null;
            }
            var manifestPaths = Directory.GetFiles(manifestsPath, "*.item", SearchOption.TopDirectoryOnly);
            foreach (var manifestPath in manifestPaths)
            {
                var json = File.ReadAllText(manifestPath);
                var epicManifest = JsonConvert.DeserializeObject<EpicManifest>(json);
                if (epicManifest.InstallLocation.Contains("OuterWilds") && IsValidGamePath(epicManifest.InstallLocation))
                {
                    return epicManifest.InstallLocation;
                }
            }
            _writer.WriteLine("Game not found in EGS.");
            return null;
        }

        private string FindGameInSteam()
        {
            var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Valve\Steam");
            var steamPath = (string)key?.GetValue("SteamPath");
            if (string.IsNullOrEmpty(steamPath))
            {
                _writer.WriteLine("Steam not found in Registry.");
                return null;
            }
            var defaultLocation = $"{steamPath}/steamapps/common/Outer Wilds";
            if (IsValidGamePath(defaultLocation))
            {
                return defaultLocation;
            }
            var libraryFoldersFile = $"{steamPath}/steamapps/libraryfolders.vdf";
            if (!File.Exists(libraryFoldersFile))
            {
                _writer.WriteLine($"Steam library folders file not found: {libraryFoldersFile}");
                return null;
            }
            var libraryFoldersContent = File.ReadAllText(libraryFoldersFile);
            for (var i = 1; i < 10; i++)
            {
                var libraryStart = $"\"{i}\"\t\t\"";
                var startIndex = libraryFoldersContent.IndexOf(libraryStart);
                if (startIndex < 0)
                {
                    _writer.WriteLine("Game not found in custom Steam library.");
                    return null;
                }
                var afterStartIndex = startIndex + libraryStart.Length;
                var endIndex = libraryFoldersContent.IndexOf('\"', afterStartIndex);
                var length = endIndex - afterStartIndex;
                var libraryPath = libraryFoldersContent.Substring(afterStartIndex, length);
                var gamePath = $"{libraryPath}/steamapps/common/Outer Wilds";
                if (IsValidGamePath(gamePath))
                {
                    return gamePath;
                }
            }
            _writer.WriteLine("Game not found in Steam.");
            return null;
        }

        private string PromptGamePath()
        {
            var gamePath = _config.GamePath;
            while (!IsValidGamePath(gamePath))
            {
                _writer.WriteLine($"Game not found at {gamePath}");
                _writer.WriteLine("Please enter the correct game path:");
                gamePath = Console.ReadLine()?.Trim();
            }
            return gamePath;
        }

        private bool IsValidGamePath(string gamePath)
        {
            return !string.IsNullOrEmpty(gamePath) &&
                   Directory.Exists(gamePath) &&
                   Directory.Exists($"{gamePath}/OuterWilds_Data/Managed") &&
                   File.Exists($"{gamePath}/OuterWilds.exe");
        }

        public class EpicManifest
        {
            [JsonProperty("InstallLocation")]
            public string InstallLocation { get; set; }
        }

    }
}
