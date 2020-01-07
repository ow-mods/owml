using System;
using System.Linq;
using System.Reflection;
using OWML.Common;
using OWML.ModHelper;
using OWML.ModHelper.Assets;
using OWML.ModHelper.Events;
using OWML.ModHelper.Menus;
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

        public Owo(IModFinder modFinder, IModLogger logger, IModConsole console, IOwmlConfig owmlConfig, IModMenus menus)
        {
            _modFinder = modFinder;
            _logger = logger;
            _console = console;
            _owmlConfig = owmlConfig;
            _menus = menus;
        }

        public void LoadMods()
        {
            if (_owmlConfig.Verbose)
            {
                _console.WriteLine("Verbose mod is enabled");
                Application.logMessageReceived += OnLogMessageReceived;
            }
            var manifests = _modFinder.GetManifests();
            foreach (var manifest in manifests)
            {
                var helper = CreateModHelper(manifest);
                LoadMod(helper);
            }
        }

        private ModsMenu CreateModMenu()
        {
            _console.WriteLine("Creating mod menu");
            var modsMenu = new ModsMenu(_logger, _console);
            var modMenuButton = _menus.MainMenu.AddButton("MODS", 3);
            modMenuButton.onClick.AddListener(() =>
            {
                modsMenu.Open();
            });
            return modsMenu;
        }

        private IModHelper CreateModHelper(IModManifest manifest)
        {
            var assets = new ModAssets(_console, manifest);
            var storage = new ModStorage(_logger, _console, manifest);
            var harmonyHelper = new HarmonyHelper(_logger, _console, manifest);
            var events = new ModEvents(_logger, _console, harmonyHelper);
            var modConfig = GetModConfig(storage);
            return new ModHelper.ModHelper(_logger, _console, harmonyHelper, events, assets, storage, _menus, manifest, modConfig, _owmlConfig);
        }

        private IModConfig GetModConfig(IModStorage storage)
        {
            _logger.Log("Initializing config");
            var config = storage.Load<ModConfig>("config.json") ?? new ModConfig();
            var defaultConfig = storage.Load<ModConfig>("default-config.json") ?? new ModConfig();
            foreach (var setting in defaultConfig.Settings)
            {
                if (!config.Settings.ContainsKey(setting.Key))
                {
                    config.Settings.Add(setting.Key, setting.Value);
                }
            }
            storage.Save(config, "config.json");
            return config;
        }

        private void OnLogMessageReceived(string message, string stackTrace, LogType type)
        {
            _logger.Log($"{type}: {message}");
            if (type == LogType.Error || type == LogType.Exception)
            {
                _console.WriteLine($"{type}: {message}");
            }
        }

        private void LoadMod(IModHelper helper)
        {
            var modType = LoadModType(helper.Manifest);
            if (modType == null)
            {
                _logger.Log("Mod type is null, skipping");
                return;
            }
            _logger.Log($"Loading {helper.Manifest.UniqueName} ({helper.Manifest.Version})...");
            _logger.Log("Adding mod behaviour...");
            var go = new GameObject(helper.Manifest.UniqueName);
            try
            {
                var mod = (ModBehaviour)go.AddComponent(modType);
                _logger.Log("Added! Initializing...");
                mod.Init(helper);
            }
            catch (Exception ex)
            {
                _console.WriteLine($"Error while adding/initializing {helper.Manifest.UniqueName}: {ex}");
                return;
            }
            _console.WriteLine($"Loaded {helper.Manifest.UniqueName} ({helper.Manifest.Version}).");
        }

        private Type LoadModType(IModManifest manifest)
        {
            if (!manifest.Enabled)
            {
                _logger.Log($"{manifest.UniqueName} is disabled");
                return null;
            }
            _logger.Log("Loading assembly: " + manifest.AssemblyPath);
            var assembly = Assembly.LoadFile(manifest.AssemblyPath);
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

    }
}
