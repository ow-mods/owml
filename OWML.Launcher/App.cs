﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using OWML.Common;
using OWML.Patcher;
using OWML.Update;

namespace OWML.Launcher
{
    public class App
    {
        private const string Version = "0.3.19";

        private readonly IOwmlConfig _owmlConfig;
        private readonly IModConsole _writer;
        private readonly IModFinder _modFinder;
        private readonly OutputListener _listener;
        private readonly PathFinder _pathFinder;
        private readonly OWPatcher _owPatcher;
        private readonly VrPatcher _vrPatcher;
        private readonly ModUpdate _update;

        public App(IOwmlConfig owmlConfig, IModConsole writer, IModFinder modFinder,
            OutputListener listener, PathFinder pathFinder, OWPatcher owPatcher, VrPatcher vrPatcher, ModUpdate update)
        {
            _owmlConfig = owmlConfig;
            _writer = writer;
            _modFinder = modFinder;
            _listener = listener;
            _pathFinder = pathFinder;
            _owPatcher = owPatcher;
            _vrPatcher = vrPatcher;
            _update = update;
        }

        public void Run()
        {
            _writer.WriteLine($"Started OWML version {Version}");
            _writer.WriteLine("For detailed log, see Logs/OWML.Log.txt");

            CheckVersion();

            LocateGamePath();

            CopyGameFiles();

            ListenForOutput();

            ShowModList();

            PatchGame();

            StartGame();

            Console.ReadLine();
        }

        private void CheckVersion()
        {
            var latestVersion = _update.GetLatestVersion();
            if (string.IsNullOrEmpty(latestVersion))
            {
                _writer.WriteLine("Could not check version.");
                return;
            }
            if (Version == latestVersion)
            {
                _writer.WriteLine("OWML is up to date.");
                return;
            }
            _writer.WriteLine($"Warning: please update OWML to {latestVersion}: {_update.ReleasesUrl}");
        }

        private void LocateGamePath()
        {
            var gamePath = _pathFinder.FindGamePath();
            _writer.WriteLine("Game found in " + gamePath);
            if (gamePath != _owmlConfig.GamePath)
            {
                _owmlConfig.GamePath = gamePath;
                SaveConfig();
            }
        }

        private void SaveConfig()
        {
            var json = JsonConvert.SerializeObject(_owmlConfig);
            File.WriteAllText("OWML.Config.json", json);
        }

        private void CopyGameFiles()
        {
            var filesToCopy = new[] { "UnityEngine.CoreModule.dll", "Assembly-CSharp.dll" };
            foreach (var fileName in filesToCopy)
            {
                File.Copy($"{_owmlConfig.ManagedPath}/{fileName}", fileName, true);
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
            var lines = s.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                _writer.WriteLine(line);
            }
        }

        private void PatchGame()
        {
            _owPatcher.PatchGame();
            _vrPatcher.PatchVR();
        }

        private void StartGame()
        {
            _writer.WriteLine("Starting game...");
            try
            {
                Process.Start($"{_owmlConfig.GamePath}/OuterWilds.exe");
            }
            catch (Exception ex)
            {
                _writer.WriteLine("Error while starting game: " + ex.Message);
            }
        }

    }
}
