using System;
using System.IO;
using Newtonsoft.Json;
using OWML.Common;
using OWML.ModHelper;
using OWML.ModHelper.Events;
using OWML.ModHelper.Logging;
using OWML.ModHelper.Menus;
using UnityEngine;

namespace OWML.ModLoader
{
    public class ModLoader
    {
        private static readonly string ConfigPath = $"{Application.dataPath}/Managed/OWML.Config.json";
        private static readonly string ManifestPath = $"{Application.dataPath}/Managed/OWML.Manifest.json";

        public static void LoadMods()
        {
            var owmlGo = new GameObject();
            owmlGo.AddComponent<OwmlBehaviour>();
            var owmlConfig = GetJsonObject<OwmlConfig>(ConfigPath);
            var owmlManifest = GetJsonObject<ModManifest>(ManifestPath);
            if (owmlConfig == null || owmlManifest == null)
            {
                // Everything is wrong and can't write to console...
                return;
            }
            var logger = new ModLogger(owmlConfig, owmlManifest);
            logger.Log("Got config!");
            var console = GetConsole(owmlConfig, logger, owmlManifest);
            console.WriteLine("Mod loader has been initialized.");
            var modFinder = new ModFinder(owmlConfig, console);
            var harmonyHelper = new HarmonyHelper(logger, console);
            var events = new ModEvents(logger, console, harmonyHelper);
            var menus = new ModMenus(logger, console, events);
            var owo = new Owo(modFinder, logger, console, owmlConfig, menus, harmonyHelper);
            owo.LoadMods();
        }

        private static IModConsole GetConsole(IOwmlConfig owmlConfig, IModLogger logger, IModManifest owmlManifest)
        {
            if (CommandLineArguments.HasArgument(Constants.ConsolePortArgument))
            {
                return new ModSocketConsole(logger, owmlManifest);
            }
            else
            {
                return new ModConsole(owmlConfig, logger, owmlManifest);
            }
        }

        private static T GetJsonObject<T>(string path)
        {
            try
            {
                var json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception)
            {
                return default(T);
            }
        }

    }
}
