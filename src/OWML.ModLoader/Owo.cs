using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OWML.Common;
using OWML.Common.Menus;
using OWML.Logging;
using OWML.ModHelper;
using OWML.ModHelper.Assets;
using OWML.ModHelper.Events;
using OWML.ModHelper.Interaction;
using OWML.Utils;

namespace OWML.ModLoader
{
	public class Owo
	{
		private readonly IModFinder _modFinder;
		private readonly IModLogger _logger;
		private readonly IModConsole _console;
		private readonly IOwmlConfig _owmlConfig;
		private readonly IModMenus _menus;
		private readonly IHarmonyHelper _harmonyHelper;
		private readonly IModInputHandler _inputHandler;
		private readonly IModSorter _sorter;
		private readonly IUnityLogger _unityLogger;
		private readonly IModSocket _socket;
		private readonly IObjImporter _objImporter;
		private readonly IGameObjectHelper _goHelper;
		private readonly IApplicationHelper _appHelper;
		private readonly IProcessHelper _processHelper;
		private readonly IModUnityEvents _unityEvents;
		private readonly IList<IModBehaviour> _modList = new List<IModBehaviour>();

		public Owo(
			IModFinder modFinder,
			IModLogger logger,
			IModConsole console,
			IOwmlConfig owmlConfig,
			IModMenus menus,
			IHarmonyHelper harmonyHelper,
			IModInputHandler inputHandler,
			IModSorter sorter,
			IUnityLogger unityLogger,
			IModSocket socket,
			IObjImporter objImporter,
			IGameObjectHelper goHelper,
			IApplicationHelper appHelper,
			IProcessHelper processHelper,
			IModUnityEvents unityEvents)
		{
			_modFinder = modFinder;
			_logger = logger;
			_console = console;
			_owmlConfig = owmlConfig;
			_menus = menus;
			_harmonyHelper = harmonyHelper;
			_inputHandler = inputHandler;
			_sorter = sorter;
			_unityLogger = unityLogger;
			_socket = socket;
			_objImporter = objImporter;
			_goHelper = goHelper;
			_appHelper = appHelper;
			_processHelper = processHelper;
			_unityEvents = unityEvents;
		}

		public void LoadMods()
		{
			_console.WriteLine("Mod loader has been initialized.");
			_console.WriteLine($"Game version: {_appHelper.Version}", MessageType.Info);

			_goHelper.CreateAndAdd<OwmlBehaviour>();

			_unityLogger.Start();
			var mods = _modFinder.GetMods();

			var changedSettings = mods.Where(mod => mod.FixConfigs()).Select(mod => mod.Manifest.Name).ToArray();
			if (changedSettings.Any())
			{
				_console.WriteLine($"Warning - Settings of following mods changed:\n\t{string.Join("\n\t", changedSettings)}", MessageType.Warning);
			}

			var normalMods = mods.Where(mod => !mod.Manifest.PriorityLoad).ToList();
			var sortedNormal = _sorter.SortMods(normalMods);

			var priorityMods = mods.Where(mod => mod.Manifest.PriorityLoad).ToList();
			var sortedPriority = _sorter.SortMods(priorityMods);

			var modNames = mods.Where(mod => mod.Config.Enabled)
				.Select(mod => mod.Manifest.UniqueName).ToList();
			var sortedMods = sortedPriority.Concat(sortedNormal);

			foreach (var modData in sortedMods)
			{
				var missingDependencies = modData.Config.Enabled
					? modData.Manifest.Dependencies.Where(dependency => !modNames.Contains(dependency)).ToList()
					: new List<string>();
				missingDependencies.ForEach(dependency => _console.WriteLine($"Error! {modData.Manifest.UniqueName} needs {dependency}, but it's disabled/missing!", MessageType.Error));
				var modType = LoadMod(modData);
				if (modType == null || missingDependencies.Any())
				{
					_menus.ModsMenu?.AddMod(modData, null);
					continue;
				}
				var helper = CreateModHelper(modData);
				var initMod = InitializeMod(modType, helper);
				_menus.ModsMenu?.AddMod(modData, initMod);
				_modList.Add(initMod);
			}
		}

		private Type LoadMod(IModData modData)
		{
			if (!modData.Config.Enabled)
			{
				_logger.Log($"{modData.Manifest.UniqueName} is disabled");
				return null;
			}

			_logger.Log($"Loading assembly: {modData.Manifest.AssemblyPath}");
			var assembly = Assembly.LoadFile(modData.Manifest.AssemblyPath);
			_logger.Log($"Loaded {assembly.FullName}");

			try
			{
				return assembly.GetTypes().FirstOrDefault(x => x.IsSubclassOf(typeof(ModBehaviour)));
			}
			catch (Exception ex)
			{
				ModConsole.OwmlConsole.WriteLine($"Error while trying to get {typeof(ModBehaviour)}: {ex.Message}", MessageType.Error);
				return null;
			}
		}

		private IModHelper CreateModHelper(IModData modData)
		{
			return new Container()
				.Add(_owmlConfig)
				.Add(modData.Manifest)
				.Add(modData.Config)
				.Add(_socket)
				.Add(_objImporter)
				.Add(_modList)
				.Add(_menus)
				.Add(_inputHandler)
				.Add(_processHelper)
				.Add(_unityEvents)
				.Add<IHarmonyHelper, HarmonyHelper>()
				.Add<IModLogger, ModLogger>()
				.Add<IModConsole, ModSocketOutput>()
				.Add<IModAssets, ModAssets>()
				.Add<IModStorage, ModStorage>()
				.Add<IModPlayerEvents, ModPlayerEvents>()
				.Add<IModSceneEvents, ModSceneEvents>()
				.Add<IModEvents, ModEvents>()
				.Add<IInterfaceProxyFactory, InterfaceProxyFactory>()
				.Add<IModInteraction, ModInteraction>()
				.Add<IModHelper, ModHelper.ModHelper>()
				.Resolve<IModHelper>();
		}

		private IModBehaviour InitializeMod(Type modType, IModHelper helper)
		{
			_logger.Log($"Initializing {helper.Manifest.UniqueName} ({helper.Manifest.Version})...");
			_logger.Log("Adding mod behaviour...");
			try
			{
				var mod = _goHelper.CreateAndAdd<IModBehaviour>(modType, helper.Manifest.UniqueName);
				_logger.Log("Added! Initializing...");
				mod.Init(helper);
				return mod;
			}
			catch (Exception ex)
			{
				_console.WriteLine($"Error while adding/initializing {helper.Manifest.UniqueName}: {ex}", MessageType.Error);
				return null;
			}
		}

	}
}
