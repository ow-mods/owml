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
            _helper.Logger.Log($"Loading {manifest.UniqueName} ({manifest.Version})...");
            var modType = _modFinder.LoadModType(manifest);
            _helper.Logger.Log("Adding mod behaviour...");
            var go = new GameObject(manifest.UniqueName);
            var mod = (ModBehaviour)go.AddComponent(modType);
            _helper.Logger.Log("Added! Initializing...");
            mod.Init(_helper);
            _helper.Console.WriteLine($"Loaded {manifest.UniqueName} ({manifest.Version}).");
            _helper.Logger.Log($"Loaded {manifest.UniqueName} ({manifest.Version}).");
        }

    }
}
