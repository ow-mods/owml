using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using OWML.Common;
using OWML.ModLoader;
using OWML.Patcher;

namespace OWML.Launcher
{
    public class App
    {
        public void Run()
        {
            Console.WriteLine("Started OWML.");

            var config = GetConfig();
            if (!Directory.Exists(config.GamePath))
            {
                Console.WriteLine($"Game not found at {config.GamePath}");
                Console.WriteLine("Edit OWML.Config.json and try again.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine($"Game found at {config.GamePath}");
            Console.WriteLine($"For detailed log, see Logs/OWML.Log.txt");

            CopyGameFiles(config);

            ListenForOutput(config);

            var modFinder = new ModFinder(config, new ModLogger(config), new ModConsole(config));

            ShowModList(modFinder);

            PatchGame(config);

            StartGame(config);

            Console.ReadLine();
        }

        private void CopyGameFiles(IModConfig config)
        {
            var filesToCopy = new[] { "UnityEngine.CoreModule.dll", "Assembly-CSharp.dll" };
            foreach (var fileName in filesToCopy)
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
                Console.WriteLine($"* {manifest.UniqueName} ({manifest.Version}){stateText}");
            }
        }

        private ModConfig GetConfig()
        {
            var json = File.ReadAllText("OWML.Config.json");
            var config = JsonConvert.DeserializeObject<ModConfig>(json);
            config.OWMLPath = AppDomain.CurrentDomain.BaseDirectory;
            return config;
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
            var filesToCopy = new[] { "OWML.ModLoader.dll", "OWML.Common.dll", "OWML.Events.dll", "Newtonsoft.Json.dll", "System.Runtime.Serialization.dll", "0Harmony.dll" };
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
