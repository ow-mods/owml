using OWML.Common.Menus;

namespace OWML.Common
{
    /// <summary>
    /// Contains helper methods for each mod.
    /// </summary>
    public interface IModHelper
    {
        /// <summary>Handler for logging to file.</summary>
        IModLogger Logger { get; }

        /// <summary>Handler for logging to console.</summary>
        IModConsole Console { get; }

        /// <summary>Handler for subscribing to events.</summary>
        IModEvents Events { get; }

        /// <summary>Handler for patching methods.</summary>
        IHarmonyHelper HarmonyHelper { get; }

        /// <summary>Handler for loading assets.</summary>
        IModAssets Assets { get; }

        /// <summary>Handler for loading / saving JSON files.</summary>
        IModStorage Storage { get; }

        /// <summary>Handler for making menus.</summary>
        IModMenus Menus { get; }

        /// <summary>The mod manifest.</summary>
        IModManifest Manifest { get; }

        /// <summary>The mod config.</summary>
        IModConfig Config { get; }

        /// <summary>OWML's config.</summary>
        IOwmlConfig OwmlConfig { get; }

        /// <summary>Handler for interacting between mods.</summary>
        IModInteraction Interaction { get; }
    }
}
