using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using OWML.Common;
using OWML.Patcher;

namespace OWML.Launcher
{
    public class App
    {
        private const string Version = "0.3.7";

        private readonly string[] _filesToCopy = { "UnityEngine.CoreModule.dll", "Assembly-CSharp.dll" };

        private readonly IModConfig _config;
        private readonly IModConsole _writer;
        private readonly IModFinder _modFinder;
        private readonly OutputListener _listener;

        public App(IModConfig config, IModConsole writer, IModFinder modFinder, OutputListener listener)
        {
            _config = config;
            _writer = writer;
            _modFinder = modFinder;
            _listener = listener;
        }

        public void Run()
        {
            _writer.WriteLine($"Started OWML version {Version}");
            _writer.WriteLine("For detailed log, see Logs/OWML.Log.txt");
            
            RequireCorrectGamePath();

            _writer.WriteLine($"Game found at {_config.GamePath}");

            CopyGameFiles();

            ListenForOutput();

            ShowModList();

            PatchGame();

            StartGame();

            Console.ReadLine();
        }

        private void RequireCorrectGamePath()
        {
            var isValidGamePath = IsValidGamePath();
            while (!isValidGamePath)
            {
                _writer.WriteLine($"Game not found at {_config.GamePath}");
                _writer.WriteLine("Please enter the correct game path:");
                _config.GamePath = Console.ReadLine()?.Trim();
                if (IsValidGamePath())
                {
                    SaveConfig();
                    isValidGamePath = true;
                }
            }
        }

        private bool IsValidGamePath()
        {
            return Directory.Exists(_config.GamePath) &&
                   Directory.Exists(_config.ManagedPath) &&
                   File.Exists($"{_config.GamePath}/OuterWilds.exe") &&
                   _filesToCopy.All(filename => File.Exists($"{_config.ManagedPath}/{filename}"));
        }

        private void CopyGameFiles()
        {
            foreach (var fileName in _filesToCopy)
            {
                File.Copy($"{_config.ManagedPath}/{fileName}", fileName, true);
            }
            _writer.WriteLine("Game files copied.");
        }

        private void ShowModList()
        {
            var manifests = _modFinder.GetManifests();
            if (!manifests.Any())
            {
                _writer.WriteLine("Warning: found no mods.");
                return;
            }
            _writer.WriteLine("Found mods:");
            foreach (var manifest in manifests)
            {
                var stateText = manifest.Enabled ? "" : " (disabled)";
                var versionText = manifest.OWMLVersion == Version ? "" : $" (Warning: made for other version of OWML: {manifest.OWMLVersion})";
                _writer.WriteLine($"* {manifest.UniqueName} ({manifest.Version}){stateText}{versionText}");
            }
        }

        private void SaveConfig()
        {
            var json = JsonConvert.SerializeObject(_config);
            File.WriteAllText("OWML.Config.json", json);
        }

        private void ListenForOutput()
        {
            _listener.OnOutput += OnOutput;
            _listener.Start();
        }

        private void OnOutput(string s)
        {
            var lines = s.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (var line in lines)
            {
                _writer.WriteLine(line);
            }
        }

        private void PatchGame()
        {
            var patcher = new ModPatcher(_config, _writer);
            patcher.PatchGame();
            var filesToCopy = new[] { "OWML.ModLoader.dll", "OWML.Common.dll", "OWML.ModHelper.dll", "OWML.ModHelper.Events.dll", "OWML.ModHelper.Assets.dll",
                "Newtonsoft.Json.dll", "System.Runtime.Serialization.dll", "0Harmony.dll", "NAudio-Unity.dll" };
            foreach (var filename in filesToCopy)
            {
                File.Copy(filename, $"{_config.ManagedPath}/{filename}", true);
            }
            File.WriteAllText($"{_config.ManagedPath}/OWML.Config.json", JsonConvert.SerializeObject(_config));
        }

        private void StartGame()
        {
            _writer.WriteLine("Starting game...");
            try
            {
                Process.Start($"{_config.GamePath}/OuterWilds.exe");
            }
            catch (Exception ex)
            {
                _writer.WriteLine("Error while starting game: " + ex.Message);
            }
        }

    }
}
