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
            if (IsValidGamePath(_config.GamePath))
            {
                return _config.GamePath;
            }
            _writer.WriteLine("Game path is not correct.");

            var gamePath = FindInDefaultFolders();
            if (!string.IsNullOrEmpty(gamePath))
            {
                return gamePath;
            }
            _writer.WriteLine("Game not found in default folders.");

            gamePath = FindInRegistry();
            if (!string.IsNullOrEmpty(gamePath))
            {
                return gamePath;
            }
            _writer.WriteLine("Game not found in registry.");

            return PromptGamePath();
        }

        private string FindInDefaultFolders()
        {
            var paths = new[]
            {
                AppDomain.CurrentDomain.BaseDirectory + "..",
                "C:/Program Files/Epic Games/OuterWilds",
                "D:/Program Files/Epic Games/OuterWilds",
                "E:/Program Files/Epic Games/OuterWilds",
                "F:/Program Files/Epic Games/OuterWilds",
                "C:/Program Files (x86)/Outer Wilds",
                "D:/Program Files (x86)/Outer Wilds",
                "E:/Program Files (x86)/Outer Wilds",
                "F:/Program Files (x86)/Outer Wilds"
            };
            return paths.FirstOrDefault(IsValidGamePath);
        }

        private string FindInRegistry()
        {
            var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Epic Games\EpicGamesLauncher");
            var appDataPath = (string)key?.GetValue("AppDataPath");
            if (string.IsNullOrEmpty(appDataPath))
            {
                return null;
            }
            var manifestsPath = appDataPath + "Manifests";
            if (!Directory.Exists(manifestsPath))
            {
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
            return null;
        }

        private string PromptGamePath()
        {
            var gamePath = _config.GamePath;
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

        internal class EpicManifest
        {
            [JsonProperty("InstallLocation")]
            public string InstallLocation { get; set; }
        }

    }
}
