using System;
using System.Collections.Generic;
using System.IO;
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
		private readonly IModConsole _console;
		private readonly IOwmlConfig _owmlConfig;
		private readonly IModMenus _menus;
		private readonly IModSorter _sorter;
		private readonly IUnityLogger _unityLogger;
		private readonly IModSocket _socket;
		private readonly IObjImporter _objImporter;
		private readonly IGameObjectHelper _goHelper;
		private readonly IApplicationHelper _appHelper;
		private readonly IProcessHelper _processHelper;
		private readonly IModUnityEvents _unityEvents;
		private IModManifest _owmlManifest;
		private readonly IList<IModBehaviour> _modList = new List<IModBehaviour>();

		public Owo(
			IModFinder modFinder,
			IModConsole console,
			IOwmlConfig owmlConfig,
			IModMenus menus,
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
			_console = console;
			_owmlConfig = owmlConfig;
			_menus = menus;
			_sorter = sorter;
			_unityLogger = unityLogger;
			_socket = socket;
			_objImporter = objImporter;
			_goHelper = goHelper;
			_appHelper = appHelper;
			_processHelper = processHelper;
			_unityEvents = unityEvents;
			_owmlManifest = JsonHelper.LoadJsonObject<ModManifest>($"{_owmlConfig.ManagedPath}/{Constants.OwmlManifestFileName}");
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

			var sortedMods = SortMods(mods);

			var modNames = mods.Where(mod => mod.Config.Enabled)
				.Select(mod => mod.Manifest.UniqueName).ToList();

			foreach (var modData in sortedMods)
			{
				var missingDependencies = modData.Config.Enabled
					? modData.Manifest.Dependencies.Where(dependency => !modNames.Contains(dependency)).ToList()
					: new List<string>();

				missingDependencies.ForEach(dependency => _console.WriteLine($"Error! {modData.Manifest.UniqueName} needs {dependency}, but it's disabled/missing!", MessageType.Error));

				var shouldLoad = CheckModVersion(modData);
				if (!shouldLoad)
				{
					continue;
				}

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

		private bool CheckModVersion(IModData data)
		{
			if (_owmlManifest == null)
			{
				_console.WriteLine("OWML Manifest is null.", MessageType.Error);
				return true;
			}

			var owmlVersion = _owmlManifest.Version;

			(int, int, int) SplitIntoInts(string version, string modname)
			{
				var split = version.Split('.');
				if (split.Length < 3)
				{
					_console.WriteLine($"Could not read OWML version of \"{modname}\" - Less than 3 digits.", MessageType.Error);
					return (0, 0, 0);
				}

				var success = true;
				success &= int.TryParse(split[0], out int int1);
				success &= int.TryParse(split[1], out int int2);
				success &= int.TryParse(split[2], out int int3);
				if (!success)
				{
					_console.WriteLine($"Could not read OWML version of \"{modname}\" - Could not parse as digits.", MessageType.Error);
					return (0, 0, 0);
				}

				return (int1, int2, int3);
			}

			var splitOwmlVersion = SplitIntoInts(owmlVersion, "OWML");
			var splitModVersion = SplitIntoInts(data.Manifest.OWMLVersion, data.Manifest.UniqueName);

			var mismatchText = $"Mismatch between OWML version expected by {data.Manifest.UniqueName} and installed OWML version." +
					$"\r\nOWML version expected by {data.Manifest.UniqueName} : {data.Manifest.OWMLVersion}" +
					$"\r\nOWML version installed : {owmlVersion}\r\n";

			if (splitOwmlVersion.Item1 != splitModVersion.Item1)
			{
				_console.WriteLine(mismatchText + $"As the mismatch affects X.~.~, this mod will not be loaded.", MessageType.Error);
				return false;
			}

			if (splitOwmlVersion.Item2 < splitModVersion.Item2)
			{
				_console.WriteLine(mismatchText + $"As the the mismatch affects ~.X.~, and the OWML version is lower, the mod will not be loaded.", MessageType.Error);
				return true;
			}

			if (splitOwmlVersion.Item2 > splitModVersion.Item2)
			{
				_console.WriteLine(mismatchText + $"As the the mismatch affects ~.X.~, and the OWML version is higher, the mod will still load.", MessageType.Warning);
				return true;
			}

			return true;
		}

		private IEnumerable<IModData> SortMods(IList<IModData> mods)
		{
			var normalMods = mods.Where(mod => !mod.Manifest.PriorityLoad).ToList();
			var sortedNormal = _sorter.SortMods(normalMods);

			var priorityMods = mods.Where(mod => mod.Manifest.PriorityLoad).ToList();
			var sortedPriority = _sorter.SortMods(priorityMods);

			var sortedMods = sortedPriority.Concat(sortedNormal);
			return sortedMods;
		}

		private Type LoadMod(IModData modData)
		{
			if (!modData.Config.Enabled)
			{
				_console.WriteLine($"{modData.Manifest.UniqueName} is disabled", MessageType.Debug);
				return null;
			}

			var assemblyPath = modData.Manifest.AssemblyPath;

			if (!File.Exists(assemblyPath))
			{
				_console.WriteLine($"Could not find DLL for {modData.Manifest.UniqueName}.", MessageType.Error);
				return null;
			}

			_console.WriteLine($"Loading assembly: {assemblyPath}", MessageType.Debug);
			var assembly = Assembly.LoadFile(assemblyPath);
			_console.WriteLine($"Loaded {assembly.FullName}", MessageType.Debug);

			try
			{
				return assembly.GetTypes().FirstOrDefault(x => x.IsSubclassOf(typeof(ModBehaviour)));
			}
			catch (ReflectionTypeLoadException ex)
			{
				_console.WriteLine($"ReflectionTypeLoadException while trying to load {nameof(ModBehaviour)} of mod {modData.Manifest.UniqueName}: {ex.Message}\n" +
								   "Top 5 LoaderExceptions:\n" +
								   $"* {string.Join("\n* ", ex.LoaderExceptions.Take(5).ToList().Select(e => e.Message).ToArray())}", MessageType.Error);
				return null;
			}
			catch (Exception ex)
			{
				_console.WriteLine($"Exception while trying to get {nameof(ModBehaviour)} of mod {modData.Manifest.UniqueName}: {ex.Message}", MessageType.Error);
				return null;
			}
		}

		private IModHelper CreateModHelper(IModData modData) =>
			new Container()
				.Add(_owmlConfig)
				.Add(modData.Manifest)
				.Add(modData.Config)
				.Add(_socket)
				.Add(_objImporter)
				.Add(_modList)
				.Add(_menus)
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

		private IModBehaviour InitializeMod(Type modType, IModHelper helper)
		{
			_console.WriteLine($"Initializing {helper.Manifest.UniqueName} ({helper.Manifest.Version})...", MessageType.Debug);
			_console.WriteLine("Adding mod behaviour...", MessageType.Debug);
			try
			{
				var mod = _goHelper.CreateAndAdd<IModBehaviour>(modType, helper.Manifest.UniqueName);
				_console.WriteLine("Added! Initializing...", MessageType.Debug);
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
