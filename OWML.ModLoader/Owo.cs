using System;
using System.Linq;
using System.Reflection;
using OWML.Common;
using OWML.ModHelper;
using OWML.ModHelper.Assets;
using UnityEngine;

namespace OWML.ModLoader
{
    internal class Owo
    {
        private readonly IModFinder _modFinder;
        private readonly IModLogger _logger;
        private readonly IModConsole _console;
        private readonly IModConfig _config;
        private readonly IHarmonyHelper _harmonyHelper;
        private readonly IModEvents _events;

        public Owo(IModFinder modFinder, IModLogger logger, IModConsole console, IModConfig config, IHarmonyHelper harmonyHelper, IModEvents events)
        {
            _modFinder = modFinder;
            _logger = logger;
            _console = console;
            _config = config;
            _harmonyHelper = harmonyHelper;
            _events = events;
        }

        public void LoadMods()
        {
            if (_config.Verbose)
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

        private IModHelper CreateModHelper(IModManifest manifest)
        {
            var assets = new ModAssets(_console, manifest);
            var storage = new ModStorage(manifest);
            return new ModHelper.ModHelper(_config, _logger, _console, _events, _harmonyHelper, assets, storage, manifest);
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
