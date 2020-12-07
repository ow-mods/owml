using OWML.Common;
using OWML.Common.Interfaces;
using OWML.Common.Interfaces.Menus;
using OWML.ModHelper;
using OWML.ModHelper.Events;
using OWML.ModHelper.Menus;
using OWML.ModHelper.Input;
using OWML.Common.Models;
using OWML.Logging;
using OWML.ModHelper.Assets;
using OWML.UnityAbstractions;
using OWML.Utils;
using UnityEngine;

namespace OWML.ModLoader
{
    public class ModLoader
    {
        public static void LoadMods()
        {
            var appHelper = new ApplicationHelper();
            var container = CreateContainer(appHelper);

            var owo = container.Resolve<Owo>();
            owo.LoadMods();
        }

        public static Container CreateContainer(IApplicationHelper appHelper)
        {
            var owmlConfig = JsonHelper.LoadJsonObject<OwmlConfig>($"{appHelper.DataPath}/Managed/{Constants.OwmlConfigFileName}");
            var owmlManifest = JsonHelper.LoadJsonObject<ModManifest>($"{appHelper.DataPath}/Managed/{Constants.OwmlManifestFileName}");

            if (owmlConfig == null || owmlManifest == null)
            {
                throw new UnityException("Can't load OWML config or manifest.");
            }

            return new Container()
                .Add(appHelper)
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
                .Add<IModUnityEvents, ModUnityEvents>()
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
                .Add<IGameObjectHelper, GameObjectHelper>()
                .Add<IProcessHelper, ProcessHelper>()
                .Add<Owo>();
        }
    }
}