using OWML.Common;
using OWML.Common.Interfaces;
using OWML.Common.Interfaces.Menus;
using OWML.ModHelper;
using OWML.ModHelper.Events;
using OWML.ModHelper.Menus;
using OWML.ModHelper.Input;
using UnityEngine;
using OWML.Common.Models;
using OWML.Logging;
using OWML.ModHelper.Assets;
using OWML.Utils;

namespace OWML.ModLoader
{
    public class ModLoader
    {
        private static readonly string ConfigPath = $"{Application.dataPath}/Managed/{Constants.OwmlConfigFileName}";
        private static readonly string ManifestPath = $"{Application.dataPath}/Managed/{Constants.OwmlManifestFileName}";

        public static void LoadMods()
        {
            var owmlGo = new GameObject();
            owmlGo.AddComponent<OwmlBehaviour>();

            var owmlConfig = JsonHelper.LoadJsonObject<OwmlConfig>(ConfigPath);
            var owmlManifest = JsonHelper.LoadJsonObject<ModManifest>(ManifestPath);
            if (owmlConfig == null || owmlManifest == null)
            {
                // Everything is wrong and can't write to console...
                return;
            }

            var owo = new Container()
                .Register<IOwmlConfig>(owmlConfig)
                .Register<IModManifest>(owmlManifest)
                .Register<IModLogger, ModLogger>()
                .Register<IModSocket, ModSocket>()
                .Register<IUnityLogger, UnityLogger>()
                .Register<IModConsole, ModSocketOutput>()
                .Register<IModSorter, ModSorter>()
                .Register<IModFinder, ModFinder>()
                .Register<IHarmonyHelper, HarmonyHelper>()
                .Register<IModPlayerEvents, ModPlayerEvents>()
                .Register<IHarmonyHelper, HarmonyHelper>()
                .Register<IModSceneEvents, ModSceneEvents>()
                .Register<IModEvents, ModEvents>()
                .Register<IModInputHandler, ModInputHandler>()
                .Register<IHarmonyHelper, HarmonyHelper>()
                .Register<IModStorage, ModStorage>()
                .Register<IModConfigMenuBase, OwmlConfigMenu>()
                .Register<IModMainMenu, ModMainMenu>()
                .Register<IModPauseMenu, ModPauseMenu>()
                .Register<IModsMenu, ModsMenu>()
                .Register<IModInputMenu, ModInputMenu>()
                .Register<IModMessagePopup, ModMessagePopup>()
                .Register<IModInputCombinationElementMenu, ModInputCombinationElementMenu>()
                .Register<IModPopupManager, ModPopupManager>()
                .Register<IModInputCombinationMenu, ModInputCombinationMenu>()
                .Register<IModMenus, ModMenus>()
                .Register<IObjImporter, ObjImporter>()
                .Register<Owo>()
                .Resolve<Owo>();

            owo.LoadMods();
        }
    }
}