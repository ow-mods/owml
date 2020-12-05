using Autofac;
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

            var builder = new ContainerBuilder(); // todo move?

            builder.RegisterInstance(owmlConfig).As<IOwmlConfig>();
            builder.RegisterInstance(owmlManifest).As<IModManifest>();

            builder.RegisterType<ModLogger>().As<IModLogger>();
            builder.RegisterType<ModSocket>().As<IModSocket>();
            builder.RegisterType<UnityLogger>().As<IUnityLogger>();
            builder.RegisterType<ModSocketOutput>().As<IModConsole>();
            builder.RegisterType<ModSorter>().As<IModSorter>();
            builder.RegisterType<ModFinder>().As<IModFinder>();
            builder.RegisterType<HarmonyHelper>().As<IHarmonyHelper>();
            builder.RegisterType<ModPlayerEvents>().As<IModPlayerEvents>();
            builder.RegisterType<ModSceneEvents>().As<IModSceneEvents>();
            builder.RegisterType<ModEvents>().As<IModEvents>();
            builder.RegisterType<ModInputHandler>().As<IModInputHandler>();
            builder.RegisterType<ModStorage>().As<IModStorage>();
            builder.RegisterType<OwmlConfigMenu>().As<IModConfigMenuBase>();
            builder.RegisterType<ModMainMenu>().As<IModMainMenu>();
            builder.RegisterType<ModPauseMenu>().As<IModPauseMenu>();
            builder.RegisterType<ModsMenu>().As<IModsMenu>();
            builder.RegisterType<ModInputMenu>().As<IModInputMenu>();
            builder.RegisterType<ModMessagePopup>().As<IModMessagePopup>();
            builder.RegisterType<ModInputCombinationElementMenu>().As<IModInputCombinationElementMenu>();
            builder.RegisterType<ModPopupManager>().As<IModPopupManager>();
            builder.RegisterType<ModInputCombinationMenu>().As<IModInputCombinationMenu>();
            builder.RegisterType<ModMenus>().As<IModMenus>();
            builder.RegisterType<ObjImporter>().As<IObjImporter>();

            builder.RegisterType<Owo>(); // todo need interface?

            var container = builder.Build();

            var owo = container.Resolve<Owo>();
            owo.LoadMods();
        }
    }
}