using OWML.Common;
using OWML.ModHelper;
using OWML.ModHelper.Events;
using OWML.ModHelper.Menus;
using OWML.ModHelper.Input;
using UnityEngine;
using OWML.ModHelper.Logging;

namespace OWML.ModLoader
{
    public class ModLoader
    {
        private static readonly string ConfigPath = $"{Application.dataPath}/Managed/{Constants.OwmlConfigFileName}";
        private static readonly string DefaultConfigPath = $"{Application.dataPath}/Managed/{Constants.OwmlDefaultConfigFileName}";
        private static readonly string ManifestPath = $"{Application.dataPath}/Managed/{Constants.OwmlManifestFileName}";

        public static void LoadMods()
        {
            var owmlGo = new GameObject();
            owmlGo.AddComponent<OwmlBehaviour>();
            var owmlConfig = JsonHelper.LoadJsonObject<OwmlConfig>(ConfigPath);
            var owmlDefaultConfig = JsonHelper.LoadJsonObject<OwmlConfig>(DefaultConfigPath);
            var owmlManifest = JsonHelper.LoadJsonObject<ModManifest>(ManifestPath);
            if (owmlConfig == null || owmlManifest == null)
            {
                // Everything is wrong and can't write to console...
                return;
            }
            var logger = new ModLogger(owmlConfig, owmlManifest);
            logger.Log("Got config!");
            var console = OutputFactory.CreateOutput(owmlConfig, logger, owmlManifest);
            console.WriteLine("Mod loader has been initialized.");
            var modSorter = new ModSorter(console);
            var modFinder = new ModFinder(owmlConfig, console);
            var harmonyHelper = new HarmonyHelper(logger, console);
            var events = new ModEvents(logger, console, harmonyHelper);
            var inputHandler = new ModInputHandler(logger, console, harmonyHelper, owmlConfig, events);
            var menus = new ModMenus(console, events, inputHandler, owmlManifest, owmlConfig, owmlDefaultConfig);
            var owo = new Owo(modFinder, logger, console, owmlConfig, menus, harmonyHelper, inputHandler, modSorter);
            owo.LoadMods();
        }

    }
}
