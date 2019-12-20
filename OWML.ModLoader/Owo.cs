using System;
using System.Linq;
using System.Reflection;
using OWML.Common;
using UnityEngine;

namespace OWML.ModLoader
{
    internal class Owo
    {
        private readonly IModHelper _helper;
        private readonly IModFinder _modFinder;

        public Owo(IModHelper helper, IModFinder modFinder)
        {
            _helper = helper;
            _modFinder = modFinder;
        }

        public void LoadMods()
        {
            _helper.Logger.Log($"{nameof(Owo)}: {nameof(LoadMods)}");
            var manifests = _modFinder.GetManifests();
            foreach (var manifest in manifests)
            {
                LoadMod(manifest);
            }
        }

        private void LoadMod(IModManifest manifest)
        {
            var modType = LoadModType(manifest);
            if (modType == null)
            {
                _helper.Logger.Log($"Mod type is null, skipping");
                return;
            }
            _helper.Logger.Log($"Loading {manifest.UniqueName} ({manifest.Version})...");
            _helper.Logger.Log("Adding mod behaviour...");
            var go = new GameObject(manifest.UniqueName);
            var mod = (ModBehaviour)go.AddComponent(modType);
            _helper.Logger.Log("Added! Initializing...");
            mod.Init(_helper);
            _helper.Console.WriteLine($"Loaded {manifest.UniqueName} ({manifest.Version}).");
            _helper.Logger.Log($"Loaded {manifest.UniqueName} ({manifest.Version}).");
        }

        private Type LoadModType(IModManifest manifest)
        {
            if (!manifest.Enabled)
            {
                _helper.Logger.Log($"{manifest.UniqueName} is disabled");
                return null;
            }
            _helper.Logger.Log("Loading assembly: " + manifest.AssemblyPath);
            var assembly = Assembly.LoadFile(manifest.AssemblyPath);
            _helper.Logger.Log($"Loaded {assembly.FullName}");
            try
            {
                return assembly.GetTypes().FirstOrDefault(x => x.IsSubclassOf(typeof(ModBehaviour)));
            }
            catch (Exception ex)
            {
                _helper.Logger.Log($"Error while trying to get {typeof(ModBehaviour)}: {ex.Message}");
                _helper.Console.WriteLine($"Error while trying to get {typeof(ModBehaviour)}: {ex.Message}");
                return null;
            }
        }

    }
}
