using OWML.Common;
using OWML.ModHelper;
using OWML.ModHelper.Events;
using OWML.ModHelper.Menus;
using OWML.ModHelper.Input;
using UnityEngine;
using OWML.Common.Models;
using OWML.Logging;
using OWML.ModHelper.Assets;

namespace OWML.ModLoader
{
    public class ModLoader
    {
        private static readonly string ConfigPath = $"{Application.dataPath}/Managed/{Constants.OwmlConfigFileName}";

        private static readonly string DefaultConfigPath = $"{Application.dataPath}/Managed/{Constants.OwmlDefaultConfigFileName}";

        private static readonly string ManifestPath = $"{Application.dataPath}/Managed/{Constants.OwmlManifestFileName}";

        public static void LoadMods() // todo DI
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
            var socket = new ModSocket(owmlConfig);
            var unityLogger = new UnityLogger(socket);
            var console = new ModSocketOutput(owmlConfig, logger, owmlManifest, socket);
            var modSorter = new ModSorter(console);
            var modFinder = new ModFinder(owmlConfig, console);
            var harmonyHelper = new HarmonyHelper(logger, console);
            var playerEvents = new ModPlayerEvents();
            var sceneEvents = new ModSceneEvents();
            var events = new ModEvents(logger, console, harmonyHelper, playerEvents, sceneEvents);
            var inputHandler = new ModInputHandler(logger, console, harmonyHelper, owmlConfig, events);
            var owmlStorage = new ModStorage(owmlManifest);
            var owmlMenu = new OwmlConfigMenu(owmlManifest, owmlConfig, owmlDefaultConfig, owmlStorage);
            var mainMenu = new ModMainMenu();
            var pauseMenu = new ModPauseMenu();
            var modsMenu = new ModsMenu(owmlMenu, inputHandler, events, owmlStorage);
            var inputMenu = new ModInputMenu();
            var messagePopup = new ModMessagePopup();
            var inputComboElementMenu = new ModInputCombinationElementMenu(inputHandler);
            var popupManager = new ModPopupManager(events, inputMenu, messagePopup, inputComboElementMenu);
            var inputComboMenu = new ModInputCombinationMenu();
            var menus = new ModMenus(events, mainMenu, pauseMenu, modsMenu, popupManager, inputComboMenu);
            var objImporter = new ObjImporter();
            var owo = new Owo(modFinder, logger, console, owmlConfig, menus, harmonyHelper,
                inputHandler, modSorter, unityLogger, socket, objImporter);

            owo.LoadMods();
        }
    }
}