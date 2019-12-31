using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using OWML.Common;
using OWML.ModHelper;
using OWML.ModLoader;
using OWML.Patcher;

namespace OWML.Launcher
{
    public class App
    {
        private const string Version = "0.3.4";

        private readonly string[] _filesToCopy = { "UnityEngine.CoreModule.dll", "Assembly-CSharp.dll" };

        public void Run()
        {
            Console.WriteLine($"Started OWML version {Version}");

            var config = GetConfig();

            RequireCorrectGamePath(config);

            Console.WriteLine($"Game found at {config.GamePath}");
            Console.WriteLine("For detailed log, see Logs/OWML.Log.txt");

            CopyGameFiles(config);

            ListenForOutput(config);

            var modFinder = new ModFinder(config);

            ShowModList(modFinder);

            PatchGame(config);

            StartGame(config);

            Console.ReadLine();
        }

        private void RequireCorrectGamePath(IModConfig config)
        {
            var isValidGamePath = IsValidGamePath(config);
            while (!isValidGamePath)
            {
                Console.WriteLine($"Game not found at {config.GamePath}");
                Console.WriteLine("Please enter the correct game path:");
                config.GamePath = Console.ReadLine()?.Trim();
                if (IsValidGamePath(config))
                {
                    SaveConfig(config);
                    isValidGamePath = true;
                }
            }
        }

        private bool IsValidGamePath(IModConfig config)
        {
            return Directory.Exists(config.GamePath) &&
                   Directory.Exists(config.ManagedPath) &&
                   File.Exists($"{config.GamePath}/OuterWilds.exe") &&
                   _filesToCopy.All(filename => File.Exists($"{config.ManagedPath}/{filename}"));
        }

        private void CopyGameFiles(IModConfig config)
        {
            foreach (var fileName in _filesToCopy)
            {
                File.Copy($"{config.ManagedPath}/{fileName}", fileName, true);
            }
            Console.WriteLine("Game files copied.");
        }

        private void ShowModList(IModFinder modFinder)
        {
            var manifests = modFinder.GetManifests();
            if (!manifests.Any())
            {
                Console.WriteLine("Found no mods.");
                return;
            }
            Console.WriteLine("Found mods:");
            foreach (var manifest in manifests)
            {
                var stateText = manifest.Enabled ? "" : " (disabled)";
                var versionText = manifest.OWMLVersion == Version ? "" : $" (Warning: made for other version of OWML: {manifest.OWMLVersion})";
                Console.WriteLine($"* {manifest.UniqueName} ({manifest.Version}){stateText}{versionText}");
            }
        }

        private IModConfig GetConfig()
        {
            var json = File.ReadAllText("OWML.Config.json")
                .Replace("\\", "/");
            var config = JsonConvert.DeserializeObject<ModConfig>(json);
            config.OWMLPath = AppDomain.CurrentDomain.BaseDirectory;
            return config;
        }

        private void SaveConfig(IModConfig config)
        {
            var json = JsonConvert.SerializeObject(config);
            File.WriteAllText("OWML.Config.json", json);
        }

        private void ListenForOutput(IModConfig config)
        {
            var listener = new OutputListener(config);
            listener.OnOutput += Console.Write;
            listener.Start();
        }

        private void PatchGame(IModConfig config)
        {
            var patcher = new ModPatcher(config);
            patcher.PatchGame();
            var filesToCopy = new[] { "OWML.ModLoader.dll", "OWML.Common.dll", "OWML.ModHelper.dll", "OWML.ModHelper.Events.dll", "OWML.ModHelper.Assets.dll",
                "Newtonsoft.Json.dll", "System.Runtime.Serialization.dll", "0Harmony.dll", "NAudio-Unity.dll" };
            foreach (var filename in filesToCopy)
            {
                File.Copy(filename, $"{config.ManagedPath}/{filename}", true);
            }
            File.WriteAllText($"{config.ManagedPath}/OWML.Config.json", JsonConvert.SerializeObject(config));
        }

        private void StartGame(IModConfig config)
        {
            Console.WriteLine("Starting game...");
            try
            {
                Process.Start($"{config.GamePath}/OuterWilds.exe");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while starting game: " + ex.Message);
            }
        }

    }
}
