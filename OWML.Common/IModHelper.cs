using OWML.Common.Menus;

namespace OWML.Common
{
    /// <summary>
    /// Contains helper methods for each mod.
    /// </summary>
    public interface IModHelper
    {
        /// <summary>Helper for logging to file.</summary>
        IModLogger Logger { get; }

        /// <summary>Helper for logging to console.</summary>
        IModConsole Console { get; }

        /// <summary>Helper for subscribing to events.</summary>
        IModEvents Events { get; }

        /// <summary>Helper for patching methods.</summary>
        IHarmonyHelper HarmonyHelper { get; }

        /// <summary>Helper for loading assets.</summary>
        IModAssets Assets { get; }

        /// <summary>Helper for loading / saving JSON files.</summary>
        IModStorage Storage { get; }

        /// <summary>Helper for making menus.</summary>
        IModMenus Menus { get; }

        /// <summary>The mod manifest.</summary>
        IModManifest Manifest { get; }

        /// <summary>The mod config.</summary>
        IModConfig Config { get; }

        /// <summary>OWML's config.</summary>
        IOwmlConfig OwmlConfig { get; }

        /// <summary>Helper for interacting between mods.</summary>
        IModInteraction Interaction { get; }
    }
}
