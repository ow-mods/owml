using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using OWML.Common;
using OWML.Common.Enums;
using OWML.Common.Interfaces;
using OWML.ModHelper;

namespace OWML.Launcher
{
    public class App
    {
        private readonly IOwmlConfig _owmlConfig;
        private readonly IModManifest _owmlManifest;
        private readonly IModConsole _writer;
        private readonly IModFinder _modFinder;
        private readonly IPathFinder _pathFinder;
        private readonly IOWPatcher _owPatcher;
        private readonly IVRPatcher _vrPatcher;
        private readonly IGameVersionHandler _versionHandler;

        public App(
            IOwmlConfig owmlConfig,
            IModManifest owmlManifest,
            IModConsole writer,
            IModFinder modFinder,
            IPathFinder pathFinder,
            IOWPatcher owPatcher,
            IVRPatcher vrPatcher,
            IGameVersionHandler versionHandler)
        {
            _owmlConfig = owmlConfig;
            _owmlManifest = owmlManifest;
            _writer = writer;
            _modFinder = modFinder;
            _pathFinder = pathFinder;
            _owPatcher = owPatcher;
            _vrPatcher = vrPatcher;
            _versionHandler = versionHandler;
        }

        public void Run(string[] args)
        {
            _writer.WriteLine($"Started OWML v{_owmlManifest.Version}", MessageType.Info);

            LocateGamePath();

            CheckGameVersion();

            CopyGameFiles();

            CreateLogsDirectory();

            var mods = _modFinder.GetMods();

            ShowModList(mods);

            PatchGame(mods);

            StartGame(args);

            var hasPortArgument = CommandLineArguments.HasArgument(Constants.ConsolePortArgument);
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
                JsonHelper.SaveJsonObject(Constants.OwmlConfigFileName, _owmlConfig);
            }
        }

        private void CheckGameVersion()
        {
            _versionHandler.CompareVersions();
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

        private void ShowModList(List<IModData> mods)
        {
            if (!mods.Any())
            {
                _writer.WriteLine("Warning - No mods found.", MessageType.Warning);
                return;
            }
            _writer.WriteLine("Found mods:");
            foreach (var modData in mods)
            {
                var stateText = modData.Enabled ? "" : "(disabled)";
                var type = modData.Enabled ? MessageType.Message : MessageType.Warning;
                _writer.WriteLine($"* {modData.Manifest.UniqueName} v{modData.Manifest.Version} {stateText}", type);
            }
        }

        private bool HasVrMod(List<IModData> mods)
        {
            var vrMod = mods.FirstOrDefault(x => x.RequireVR && x.Enabled);
            var hasVrMod = vrMod != null;
            _writer.WriteLine(hasVrMod ? $"{vrMod.Manifest.UniqueName} requires VR." : "No mods require VR.");
            return hasVrMod;
        }

        private void PatchGame(List<IModData> mods)
        {
            _owPatcher.PatchGame();

            try
            {
                var enableVR = HasVrMod(mods);
                _vrPatcher.PatchVR(enableVR);
            }
            catch (Exception ex)
            {
                _writer.WriteLine($"Error while applying VR patch: {ex}", MessageType.Error);
            }
        }

        private void StartGame(string[] args)
        {
            _writer.WriteLine("Starting game...");

            if (args.Contains("-consolePort"))
            {
                var index = Array.IndexOf(args, "-consolePort");
                var list = new List<string>(args);
                list.RemoveRange(index, 2);
                args = list.ToArray();
            }

            try
            {
                Process.Start($"{_owmlConfig.GamePath}/OuterWilds.exe", string.Join(" ", args));
            }
            catch (Exception ex)
            {
                _writer.WriteLine($"Error while starting game: {ex.Message}", MessageType.Error);
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
