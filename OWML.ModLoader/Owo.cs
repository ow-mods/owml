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
        private readonly ModSorter _sorter;

        List<ModBehaviour> _modList = new List<ModBehaviour>();

        public Owo(IModFinder modFinder, IModLogger logger, IModConsole console,
            IOwmlConfig owmlConfig, IModMenus menus, IHarmonyHelper harmonyHelper, ModSorter sorter)
        {
            _modFinder = modFinder;
            _logger = logger;
            _console = console;
            _owmlConfig = owmlConfig;
            _menus = menus;
            _harmonyHelper = harmonyHelper;
            _sorter = sorter;
        }

        public void LoadMods()
        {
            if (_owmlConfig.Verbose)
            {
                _console.WriteLine("Verbose mode is enabled");
                Application.logMessageReceived += OnLogMessageReceived;
            }
            var mods = _modFinder.GetMods();

            var sortedMods = _sorter.SortMods(mods);

            var modNames = new List<string>();
            foreach (var mod in mods)
            {
                if (mod.Config.Enabled == true)
                {
                    modNames.Add(mod.Manifest.Name);
                }
            }

            foreach (var modDep in sortedMods)
            {
                foreach (var dep in modDep.Dependencies)
                {
                    if (!modNames.Contains(dep) && dep != "None")
                    {
                        _console.WriteLine("Error! " + modDep.Name + " needs " + dep + ",  but it's disabled!");
                    }
                }
                var modData = modDep.Data;
                var modType = LoadMod(modData);
                if (modType == null)
                {
                    _logger.Log("Mod type is null, skipping");
                    _menus.ModsMenu.AddMod(modData, null);
                    continue;
                }
                var helper = CreateModHelper(modData);
                var mod = InitializeMod(modType, helper);
                _menus.ModsMenu.AddMod(modData, mod);
                _modList.Add(mod as ModBehaviour);
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
            var logger = new ModLogger(_owmlConfig, modData.Manifest);
            var console = new ModConsole(_owmlConfig, _logger, modData.Manifest);
            var assets = new ModAssets(console, modData.Manifest);
            var storage = new ModStorage(console, modData.Manifest);
            var events = new ModEvents(logger, console, _harmonyHelper);
            var interaction = new ModInteraction(_modList, _modFinder);
            return new ModHelper.ModHelper(logger, console, _harmonyHelper,
                events, assets, storage, _menus, modData.Manifest, modData.Config, _owmlConfig, interaction);
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
