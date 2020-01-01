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
        private const string Version = "0.3.7";

        private readonly string[] _filesToCopy = { "UnityEngine.CoreModule.dll", "Assembly-CSharp.dll" };

        public void Run()
        {
            PrintLine($"Started OWML version {Version}");

            var config = GetConfig();

            RequireCorrectGamePath(config);

            PrintLine($"Game found at {config.GamePath}");
            PrintLine("For detailed log, see Logs/OWML.Log.txt");

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
                PrintLine($"Game not found at {config.GamePath}");
                PrintLine("Please enter the correct game path:");
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
            PrintLine("Game files copied.");
        }

        private void ShowModList(IModFinder modFinder)
        {
            var manifests = modFinder.GetManifests();
            if (!manifests.Any())
            {
                PrintLine("Warning: found no mods.");
                return;
            }
            PrintLine("Found mods:");
            foreach (var manifest in manifests)
            {
                var stateText = manifest.Enabled ? "" : " (disabled)";
                var versionText = manifest.OWMLVersion == Version ? "" : $" (Warning: made for other version of OWML: {manifest.OWMLVersion})";
                PrintLine($"* {manifest.UniqueName} ({manifest.Version}){stateText}{versionText}");
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
            listener.OnOutput += OnOutput;
            listener.Start();
        }

        private string temp = "";

        private void OnOutput(string s)
        {
            var lines = s.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
            var lastLine = lines.Last();
            if (!string.IsNullOrEmpty(lastLine))
            {
                temp = lastLine;
                lines.Remove(lastLine);
            }
            else
            {
                lines[0] = temp + lines[0];
                temp = "";
            }
            foreach (var line in lines)
            {
                PrintLine(line);
            }
        }

        private void PrintLine(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return;
            }
            if (line.ToLower().StartsWith("error") || line.ToLower().StartsWith("exception"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else if (line.ToLower().StartsWith("warning"))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            else if (line.ToLower().StartsWith("success"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            Console.WriteLine(line);
            Console.ForegroundColor = ConsoleColor.Gray;
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
            PrintLine("Starting game...");
            try
            {
                Process.Start($"{config.GamePath}/OuterWilds.exe");
            }
            catch (Exception ex)
            {
                PrintLine("Error while starting game: " + ex.Message);
            }
        }

    }
}
