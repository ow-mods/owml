using OWML.Common;
using OWML.ModHelper;
using OWML.ModHelper.Events;
using OWML.Logging;
using OWML.ModHelper.Assets;
using OWML.Abstractions;
using OWML.Common.Menus;
using OWML.ModHelper.Menus;
using OWML.Utils;
using UnityEngine;

namespace OWML.ModLoader
{
	public class ModLoader
	{
		public static void LoadMods()
		{
			var appHelper = new ApplicationHelper();
			var goHelper = new GameObjectHelper();
			var container = CreateContainer(appHelper, goHelper);

			var owo = container.Resolve<Owo>();
			owo.LoadMods();
		}

		public static Container CreateContainer(IApplicationHelper appHelper, IGameObjectHelper goHelper)
		{
			var configPath = $"{appHelper.DataPath}/Managed/{Constants.OwmlConfigFileName}";
			var owmlConfig = JsonHelper.LoadJsonObject<OwmlConfig>(configPath);
			if (owmlConfig == null)
			{
				throw new UnityException($"Can't load OWML config: {configPath}");
			}

			var manifestPath = $"{appHelper.DataPath}/Managed/{Constants.OwmlManifestFileName}";
			var owmlManifest = JsonHelper.LoadJsonObject<ModManifest>(manifestPath);
			if (owmlManifest == null)
			{
				throw new UnityException($"Can't load OWML manifest: {manifestPath}");
			}

			var unityEvents = goHelper.CreateAndAdd<IModUnityEvents, ModUnityEvents>();

			return new Container()
				.Add(appHelper)
				.Add(goHelper)
				.Add(unityEvents)
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
				.Add<IModSceneEvents, ModSceneEvents>()
				.Add<IModEvents, ModEvents>()
				.Add<IModStorage, ModStorage>()
				.Add<IModConfigMenuBase, OwmlConfigMenu>()
				.Add<IModTabbedMenu, ModOptionsMenu>()
				.Add<IModMainMenu, ModMainMenu>()
				.Add<IModPauseMenu, ModPauseMenu>()
				.Add<IModsMenu, ModsMenu>()
				.Add<IModInputMenu, ModInputMenu>()
				.Add<IModMessagePopup, ModMessagePopup>()
				.Add<IModPopupManager, ModPopupManager>()
				.Add<IModMenus, ModMenus>()
				.Add<IObjImporter, ObjImporter>()
				.Add<IProcessHelper, ProcessHelper>()
				.Add<IModVersionChecker, ModVersionChecker>()
				.Add<Owo>();
		}
	}
}