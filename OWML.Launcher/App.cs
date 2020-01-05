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
        private const string Version = "0.3.13";

        private readonly IOwmlConfig _config;
        private readonly IModConsole _writer;
        private readonly IModFinder _modFinder;
        private readonly OutputListener _listener;
        private readonly PathFinder _pathFinder;

        public App(IOwmlConfig config, IModConsole writer, IModFinder modFinder, OutputListener listener, PathFinder pathFinder)
        {
            _config = config;
            _writer = writer;
            _modFinder = modFinder;
            _listener = listener;
            _pathFinder = pathFinder;
        }

        public void Run()
        {
            _writer.WriteLine($"Started OWML version {Version}");
            _writer.WriteLine("For detailed log, see Logs/OWML.Log.txt");

            LocateGamePath();

            CopyGameFiles();

            ListenForOutput();

            ShowModList();

            PatchGame();

            StartGame();

            Console.ReadLine();
        }

        private void LocateGamePath()
        {
            var gamePath = _pathFinder.FindGamePath();
            if (gamePath != _config.GamePath)
            {
                _config.GamePath = gamePath;
                SaveConfig();
            }
        }

        private void SaveConfig()
        {
            var json = JsonConvert.SerializeObject(_config);
            File.WriteAllText("OWML.Config.json", json);
        }

        private void CopyGameFiles()
        {
            var filesToCopy = new[] { "UnityEngine.CoreModule.dll", "Assembly-CSharp.dll" };
            foreach (var fileName in filesToCopy)
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
                var versionText = manifest.OWMLVersion == Version ? "" : $" (warning: made for OWML {manifest.OWMLVersion})";
                _writer.WriteLine($"* {manifest.UniqueName} ({manifest.Version}){stateText}{versionText}");
            }
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
            var filesToCopy = new[] { "OWML.ModLoader.dll", "OWML.Common.dll", "OWML.ModHelper.dll",
                "OWML.ModHelper.Events.dll", "OWML.ModHelper.Assets.dll", "OWML.ModHelper.Menus.dll",
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
