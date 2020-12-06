using System;
using System.Collections.Generic;
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
        private readonly IProcessHelper _processHelper;
        private readonly IArgumentHelper _argumentHelper;

        public App(
            IOwmlConfig owmlConfig,
            IModManifest owmlManifest,
            IModConsole writer,
            IModFinder modFinder,
            IPathFinder pathFinder,
            IOWPatcher owPatcher,
            IVRPatcher vrPatcher,
            IGameVersionHandler versionHandler,
            IProcessHelper processHelper,
            IArgumentHelper argumentHelper)
        {
            _owmlConfig = owmlConfig;
            _owmlManifest = owmlManifest;
            _writer = writer;
            _modFinder = modFinder;
            _pathFinder = pathFinder;
            _owPatcher = owPatcher;
            _vrPatcher = vrPatcher;
            _versionHandler = versionHandler;
            _processHelper = processHelper;
            _argumentHelper = argumentHelper;
        }

        public void Run()
        {
            _writer.WriteLine($"Started OWML v{_owmlManifest.Version}", MessageType.Info);

            LocateGamePath();

            CheckGameVersion();

            CopyGameFiles();

            CreateLogsDirectory();

            var mods = _modFinder.GetMods();

            ShowModList(mods);

            PatchGame(mods);

            var hasPortArgument = _argumentHelper.HasArgument(Constants.ConsolePortArgument);

            StartGame();

            if (hasPortArgument)
            {
                ExitConsole();
                return;
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

        private void StartGame()
        {
            _writer.WriteLine("Starting game...");

            _argumentHelper.RemoveArgument("consolePort");

            try
            {
                _processHelper.Start($"{_owmlConfig.GamePath}/OuterWilds.exe", _argumentHelper.Arguments);
            }
            catch (Exception ex)
            {
                _writer.WriteLine($"Error while starting game: {ex.Message}", MessageType.Error);
            }
        }

        private void ExitConsole()
        {
            _processHelper.KillCurrentProcess();
            //Environment.Exit(0); todo change back?
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
