
using OWML.Common;

namespace OWML.ModHelper
{
    public class ModHelper : IModHelper
    {
        public IModConfig Config { get; }
        public IModLogger Logger { get; }
        public IModConsole Console { get; }
        public IModEvents Events { get; }
        public IHarmonyHelper HarmonyHelper { get; }
        public IModAssets Assets { get; }
        public IModStorage Storage { get; }
        public IModManifest Manifest { get; }

        public ModHelper(IModConfig config, IModLogger logger, IModConsole console, IModEvents events, IHarmonyHelper harmonyHelper, IModAssets assets, IModStorage storage, IModManifest manifest)
        {
            Config = config;
            Logger = logger;
            Console = console;
            Events = events;
            HarmonyHelper = harmonyHelper;
            Assets = assets;
            Storage = storage;
            Manifest = manifest;
        }

    }
}
