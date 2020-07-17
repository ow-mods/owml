using System.IO;
using Microsoft.Win32;
using Newtonsoft.Json;
using OWML.Common;

namespace OWML.GameFinder
{
    public class EpicGameFinder : BaseFinder
    {
        private const string RegistryPath = @"SOFTWARE\WOW6432Node\Epic Games\EpicGamesLauncher";
        private const string RegistryName = "AppDataPath";
        private const string ManifestsFolder = "Manifests";
        private const string ManifestPattern = "*.item";
        private const string ManifestGameName = "OuterWilds";

        public EpicGameFinder(IOwmlConfig config, IModConsole writer) : base(config, writer)
        {
        }

        public override string FindGamePath()
        {
            var key = Registry.LocalMachine.OpenSubKey(RegistryPath);
            var appDataPath = (string)key?.GetValue(RegistryName);
            if (string.IsNullOrEmpty(appDataPath))
            {
                Writer.WriteLine(MessageType.Message, "EGS not found in registry.");
                return null;
            }
            var manifestsPath = $"{appDataPath}{ManifestsFolder}";
            if (!Directory.Exists(manifestsPath))
            {
                Writer.WriteLine(MessageType.Message, $"EGS manifests folder not found: {manifestsPath}");
                return null;
            }
            var manifestPaths = Directory.GetFiles(manifestsPath, ManifestPattern, SearchOption.TopDirectoryOnly);
            foreach (var manifestPath in manifestPaths)
            {
                var json = File.ReadAllText(manifestPath);
                var epicManifest = JsonConvert.DeserializeObject<EpicManifest>(json);
                if (epicManifest.InstallLocation.Contains(ManifestGameName) && IsValidGamePath(epicManifest.InstallLocation))
                {
                    return epicManifest.InstallLocation;
                }
            }
            Writer.WriteLine(MessageType.Message, "Game not found in EGS.");
            return null;
        }

        private class EpicManifest
        {
            [JsonProperty("InstallLocation")]
            public string InstallLocation { get; set; }
        }
    }
}
