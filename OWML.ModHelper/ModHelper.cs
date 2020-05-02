
using OWML.Common;
using OWML.Common.Menus;

namespace OWML.ModHelper
{
    public class ModHelper : IModHelper
    {
        public IModLogger Logger { get; }
        public IModConsole Console { get; }
        public IHarmonyHelper HarmonyHelper { get; }
        public IModEvents Events { get; }
        public IModAssets Assets { get; }
        public IModStorage Storage { get; }
        public IModMenus Menus { get; }
        public IModManifest Manifest { get; }
        public IModConfig Config { get; }
        public IOwmlConfig OwmlConfig { get; }
        public IModInputHandler Input { get; }

        public ModHelper(IModLogger logger, IModConsole console, IHarmonyHelper harmonyHelper, IModEvents events, 
            IModAssets assets, IModStorage storage, IModMenus menus, IModManifest manifest, IModConfig config, IOwmlConfig owmlConfig, IModInputHandler inputHandler)
        {
            Logger = logger;
            Console = console;
            HarmonyHelper = harmonyHelper;
            Events = events;
            Assets = assets;
            Storage = storage;
            Menus = menus;
            Manifest = manifest;
            Config = config;
            OwmlConfig = owmlConfig;
            Input = inputHandler;

        }

    }
}
