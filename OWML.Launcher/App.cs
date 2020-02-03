using System;
using System.Collections.Generic;
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
        private const string Version = "0.3.32";

        private readonly IOwmlConfig _owmlConfig;
        private readonly IModConsole _writer;
        private readonly IModFinder _modFinder;
        private readonly OutputListener _listener;
        private readonly PathFinder _pathFinder;
        private readonly OWPatcher _owPatcher;
        private readonly VRPatcher _vrPatcher;
        private readonly CheckVersion _checkVersion;

        public App(IOwmlConfig owmlConfig, IModConsole writer, IModFinder modFinder,
            OutputListener listener, PathFinder pathFinder, OWPatcher owPatcher, VRPatcher vrPatcher, CheckVersion checkVersion)
        {
            _owmlConfig = owmlConfig;
            _writer = writer;
            _modFinder = modFinder;
            _listener = listener;
            _pathFinder = pathFinder;
            _owPatcher = owPatcher;
            _vrPatcher = vrPatcher;
            _checkVersion = checkVersion;
        }

        public void Run(string[] args)
        {
            _writer.WriteLine($"Started OWML v{Version}");

            CheckVersion();

            _writer.WriteLine("For detailed log, see Logs/OWML.Log.txt");

            LocateGamePath();

            CopyGameFiles();

            ListenForOutput();

            var mods = _modFinder.GetMods();

            ShowModList(mods);

            PatchGame(mods);

            StartGame(args);

            Console.ReadLine();
        }

        private void CheckVersion()
        {
            _writer.WriteLine("Checking latest version...");
            var latestVersion = _checkVersion.GetOwmlVersion();
            if (string.IsNullOrEmpty(latestVersion))
            {
                _writer.WriteLine("Warning: could not check latest OWML version.");
                return;
            }
            if (Version == latestVersion)
            {
                _writer.WriteLine("OWML is up to date.");
                return;
            }
            _writer.WriteLine($"Warning: please update OWML to v{latestVersion}: {_checkVersion.GitHubReleasesUrl}");
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

        private void ShowModList(IList<IModData> mods)
        {
            if (!mods.Any())
            {
                _writer.WriteLine("Warning: found no mods.");
                return;
            }
            _writer.WriteLine("Found mods:");
            foreach (var modData in mods)
            {
                var enabled = modData.Config.Enabled && modData.Manifest.Enabled;
                var stateText = enabled ? "" : "(disabled)";
                _writer.WriteLine($"* {modData.Manifest.UniqueName} v{modData.Manifest.Version} {stateText}");

                var latestVersion = GetNexusVersion(modData.Manifest);
                if (!string.IsNullOrEmpty(latestVersion) && modData.Manifest.Version != latestVersion)
                {
                    _writer.WriteLine($"  Warning: please update to v{latestVersion}: {_checkVersion.NexusModsUrl}{modData.Manifest.AppIds["nexus"]}");
                }

                if (!string.IsNullOrEmpty(modData.Manifest.OWMLVersion) && !IsMadeForSameOwmlMajorVersion(modData.Manifest))
                {
                    _writer.WriteLine($"  Warning: made for old version of OWML: v{modData.Manifest.OWMLVersion}");
                }
            }
        }

        private bool IsMadeForSameOwmlMajorVersion(IModManifest manifest)
        {
            var owmlVersionSplit = Version.Split('.');
            var modVersionSplit = manifest.OWMLVersion.Split('.');
            return owmlVersionSplit.Length == modVersionSplit.Length &&
                   owmlVersionSplit[0] == modVersionSplit[0] &&
                   owmlVersionSplit[1] == modVersionSplit[1];
        }

        private string GetNexusVersion(IModManifest manifest)
        {
            if (manifest.AppIds == null || !manifest.AppIds.Any())
            {
                return null;
            }
            if (!manifest.AppIds.TryGetValue("nexus", out var appId))
            {
                return null;
            }
            return _checkVersion.GetNexusVersion(appId);
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
                if (line == Constants.QuitKeyPhrase)
                {
                    Environment.Exit(0);
                }
            }
        }

        private void PatchGame(IList<IModData> mods)
        {
            _owPatcher.PatchGame();

            var vrMod = mods.FirstOrDefault(x => x.Config.RequireVR && x.Config.Enabled);
            var enableVR = vrMod != null;
            _writer.WriteLine(enableVR ? $"{vrMod.Manifest.UniqueName} requires VR." : "No mods require VR.");
            _vrPatcher.PatchVR(enableVR);
        }

        private void StartGame(string[] args)
        {
            _writer.WriteLine("Starting game...");
            try
            {
                Process.Start($"{_owmlConfig.GamePath}/OuterWilds.exe", string.Join(" ", args));
            }
            catch (Exception ex)
            {
                _writer.WriteLine("Error while starting game: " + ex.Message);
            }
        }

    }
}
