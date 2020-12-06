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
                .Add<IOwmlConfig>(owmlConfig)
                .Add<IModManifest>(owmlManifest)
                .Add<IModLogger, ModLogger>()
                .Add<IModSocket, ModSocket>()
                .Add<IUnityLogger, UnityLogger>()
                .Add<IModConsole, ModSocketOutput>()
                .Add<IModSorter, ModSorter>()
                .Add<IModFinder, ModFinder>()
                .Add<IHarmonyHelper, HarmonyHelper>()
                .Add<IModPlayerEvents, ModPlayerEvents>()
                .Add<IHarmonyHelper, HarmonyHelper>()
                .Add<IModSceneEvents, ModSceneEvents>()
                .Add<IModEvents, ModEvents>()
                .Add<IModInputHandler, ModInputHandler>()
                .Add<IHarmonyHelper, HarmonyHelper>()
                .Add<IModStorage, ModStorage>()
                .Add<IModConfigMenuBase, OwmlConfigMenu>()
                .Add<IModMainMenu, ModMainMenu>()
                .Add<IModPauseMenu, ModPauseMenu>()
                .Add<IModsMenu, ModsMenu>()
                .Add<IModInputMenu, ModInputMenu>()
                .Add<IModMessagePopup, ModMessagePopup>()
                .Add<IModInputCombinationElementMenu, ModInputCombinationElementMenu>()
                .Add<IModPopupManager, ModPopupManager>()
                .Add<IModInputCombinationMenu, ModInputCombinationMenu>()
                .Add<IModMenus, ModMenus>()
                .Add<IObjImporter, ObjImporter>()
                .Add<Owo>()
                .Resolve<Owo>();

            owo.LoadMods();
        }
    }
}