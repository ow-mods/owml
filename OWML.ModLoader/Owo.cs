using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OWML.Common;
using OWML.Common.Menus;
using OWML.ModHelper;
using OWML.ModHelper.Assets;
using OWML.ModHelper.Events;
using OWML.ModHelper.Interaction;
using UnityEngine;

namespace OWML.ModLoader
{
    internal class Owo
    {
        private readonly IModFinder _modFinder;
        private readonly IModLogger _logger;
        private readonly IModConsole _console;
        private readonly IOwmlConfig _owmlConfig;
        private readonly IModMenus _menus;
        private readonly IHarmonyHelper _harmonyHelper;
        private readonly IModInputHandler _inputHandler;
        private readonly ModSorter _sorter;
        private readonly string _logFileName;

        private readonly List<IModBehaviour> _modList = new List<IModBehaviour>();

        public Owo(IModFinder modFinder, IModLogger logger, IModConsole console,
            IOwmlConfig owmlConfig, IModMenus menus, IHarmonyHelper harmonyHelper,
            IModInputHandler inputHandler, ModSorter sorter, string logFileName)
        {
            _modFinder = modFinder;
            _logger = logger;
            _console = console;
            _owmlConfig = owmlConfig;
            _menus = menus;
            _harmonyHelper = harmonyHelper;
            _inputHandler = inputHandler;
            _sorter = sorter;
            _logFileName = logFileName;
        }

        public void LoadMods()
        {
            if (_owmlConfig.Verbose)
            {
                _console.WriteLine("Verbose mode is enabled");
                Application.logMessageReceived += OnLogMessageReceived;
            }
            var mods = _modFinder.GetMods();
            mods.ForEach(mod => mod.FixConfigs());

            var normalMods = mods.Where(mod => !mod.Manifest.PriorityLoad).ToList();
            var sortedNormal = _sorter.SortMods(normalMods);

            var priorityMods = mods.Where(mod => mod.Manifest.PriorityLoad).ToList();
            var sortedPriority = _sorter.SortMods(priorityMods);

            var modNames = mods.Where(mod => mod.Config.Enabled)
                .Select(mod => mod.Manifest.UniqueName).ToList();
            var sortedMods = sortedPriority.Concat(sortedNormal);

            foreach (var modData in sortedMods)
            {
                var missingDependencies = modData.Config.Enabled ?
                    modData.Manifest.Dependencies.Where(dependency => !modNames.Contains(dependency)).ToList() :
                    new List<string>();
                missingDependencies.ForEach(dependency => _console.WriteLine(
                    $"Error! {modData.Manifest.UniqueName} needs {dependency}, but it's disabled/missing!"));
                var modType = LoadMod(modData);
                if (modType == null || missingDependencies.Any())
                {
                    _menus.ModsMenu.AddMod(modData, null);
                    continue;
                }
                var helper = CreateModHelper(modData);
                var initMod = InitializeMod(modType, helper);
                _menus.ModsMenu.AddMod(modData, initMod);
                _modList.Add(initMod);
            }
        }

        private void OnLogMessageReceived(string message, string stackTrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception)
            {
                _console.WriteLine($"Unity log message: {message}. Stack trace: {stackTrace?.Trim()}");
            }
        }

        private Type LoadMod(IModData modData)
        {
            if (!modData.Config.Enabled)
            {
                _logger.Log($"{modData.Manifest.UniqueName} is disabled");
                return null;
            }
            _logger.Log("Loading assembly: " + modData.Manifest.AssemblyPath);
            var assembly = Assembly.LoadFile(modData.Manifest.AssemblyPath);
            _logger.Log($"Loaded {assembly.FullName}");
            try
            {
                return assembly.GetTypes().FirstOrDefault(x => x.IsSubclassOf(typeof(ModBehaviour)));
            }
            catch (Exception ex)
            {
                _console.WriteLine($"Error while trying to get {typeof(ModBehaviour)}: {ex.Message}");
                return null;
            }
        }

        private IModHelper CreateModHelper(IModData modData)
        {
            var logger = new ModLogger(_owmlConfig, modData.Manifest, _logFileName);
            var console = new ModSocketOutput(_owmlConfig, _logger, modData.Manifest);
            var assets = new ModAssets(console, modData.Manifest);
            var storage = new ModStorage(modData.Manifest);
            var events = new ModEvents(logger, console, _harmonyHelper);
            var interaction = new ModInteraction(_modList, new InterfaceProxyFactory(), modData.Manifest);
            return new ModHelper.ModHelper(logger, console, _harmonyHelper,
                events, assets, storage, _menus, modData.Manifest, modData.Config,
                _owmlConfig, _inputHandler, interaction);
        }

        private IModBehaviour InitializeMod(Type modType, IModHelper helper)
        {
            _logger.Log($"Initializing {helper.Manifest.UniqueName} ({helper.Manifest.Version})...");
            _logger.Log("Adding mod behaviour...");
            var go = new GameObject(helper.Manifest.UniqueName);
            try
            {
                var mod = (ModBehaviour)go.AddComponent(modType);
                _logger.Log("Added! Initializing...");
                mod.Init(helper);
                return mod;
            }
            catch (Exception ex)
            {
                _console.WriteLine($"Error while adding/initializing {helper.Manifest.UniqueName}: {ex}");
                return null;
            }
        }

    }
}
