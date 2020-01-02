
using OWML.Common;

namespace OWML.ModHelper
{
    public class ModHelper : IModHelper
    {
        public IModConfig Config { get; }
        public IModLogger Logger { get; }
        public IModConsole Console { get; }
        public IHarmonyHelper HarmonyHelper { get; }
        public IModEvents Events { get; }
        public IModAssets Assets { get; }
        public IModStorage Storage { get; }
        public IModManifest Manifest { get; }

        public ModHelper(IModConfig config, IModLogger logger, IModConsole console, IHarmonyHelper harmonyHelper, IModEvents events,  IModAssets assets, IModStorage storage, IModManifest manifest)
        {
            Config = config;
            Logger = logger;
            Console = console;
            HarmonyHelper = harmonyHelper;
            Events = events;
            Assets = assets;
            Storage = storage;
            Manifest = manifest;
        }

    }
}
