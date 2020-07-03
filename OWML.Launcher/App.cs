using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using OWML.Common;
using OWML.GameFinder;
using OWML.Patcher;

namespace OWML.Launcher
{
    public class App
    {
        private readonly IOwmlConfig _owmlConfig;
        private readonly IModManifest _owmlManifest;
        private readonly IModConsole _writer;
        private readonly IModFinder _modFinder;
        private readonly OutputListener _listener;
        private readonly PathFinder _pathFinder;
        private readonly OWPatcher _owPatcher;
        private readonly VRPatcher _vrPatcher;

        public App(IOwmlConfig owmlConfig, IModManifest owmlManifest, IModConsole writer, IModFinder modFinder,
            OutputListener listener, PathFinder pathFinder, OWPatcher owPatcher, VRPatcher vrPatcher)
        {
            _owmlConfig = owmlConfig;
            _owmlManifest = owmlManifest;
            _writer = writer;
            _modFinder = modFinder;
            _listener = listener;
            _pathFinder = pathFinder;
            _owPatcher = owPatcher;
            _vrPatcher = vrPatcher;
        }

        public void Run(string[] args)
        {
            _writer.WriteLine($"Started OWML v{_owmlManifest.Version}");
            _writer.WriteLine("For detailed log, see Logs/OWML.Log.txt");

            LocateGamePath();

            CopyGameFiles();

            CreateLogsDirectory();

            var hasPortArgument = CommandLineArguments.HasArgument(Constants.ConsolePortArgument);
            if (!hasPortArgument)
            {
                ListenForOutput();
            }

            var mods = _modFinder.GetMods();

            ShowModList(mods);

            PatchGame(mods);

            StartGame(args);

            if (hasPortArgument)
            {
                ExitConsole();
            }

            Console.ReadLine();
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
                var stateText = modData.Config.Enabled ? "" : "(disabled)";
                _writer.WriteLine($"* {modData.Manifest.UniqueName} v{modData.Manifest.Version} {stateText}");

                if (!string.IsNullOrEmpty(modData.Manifest.OWMLVersion) && !IsMadeForSameOwmlMajorVersion(modData.Manifest))
                {
                    _writer.WriteLine($"  Warning: made for old version of OWML: v{modData.Manifest.OWMLVersion}");
                }
            }
        }

        private bool IsMadeForSameOwmlMajorVersion(IModManifest manifest)
        {
            var owmlVersionSplit = _owmlManifest.Version.Split('.');
            var modVersionSplit = manifest.OWMLVersion.Split('.');
            return owmlVersionSplit.Length == modVersionSplit.Length &&
                   owmlVersionSplit[0] == modVersionSplit[0] &&
                   owmlVersionSplit[1] == modVersionSplit[1];
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
                if (line.EndsWith(Constants.QuitKeyPhrase))
                {
                    ExitConsole();
                }
            }
        }

        private bool HasVrMod(IList<IModData> mods)
        {
            var vrMod = mods.FirstOrDefault(x => x.Config.RequireVR && x.Config.Enabled);
            var hasVrMod = vrMod != null;
            _writer.WriteLine(hasVrMod ? $"{vrMod.Manifest.UniqueName} requires VR." : "No mods require VR.");
            return hasVrMod;
        }

        private void PatchGame(IList<IModData> mods)
        {
            _owPatcher.PatchGame();

            try
            {
                var enableVR = HasVrMod(mods);
                _vrPatcher.PatchVR(enableVR);
            }
            catch (Exception ex)
            {
                _writer.WriteLine($"Error while applying VR patch: {ex}");
            }
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

        private void ExitConsole()
        {
            Environment.Exit(0);
        }

        private void CreateLogsDirectory()
        {
            if (!Directory.Exists("Logs"))
            {
                Directory.CreateDirectory("Logs");
            }
        }
    }
}
