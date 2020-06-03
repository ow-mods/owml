namespace OWML.Common
{
    /// <summary>
    /// Holds data on each mod.
    /// </summary>
    public interface IModData
    {
        /// <summary>The mod manifest.</summary>
        IModManifest Manifest { get; }

        /// <summary>The mod config.</summary>
        IModConfig Config { get; }

        /// <summary>The default mod config.</summary>
        IModConfig DefaultConfig { get; }

        /// <summary>Resets the config back to default settings.</summary>
        void ResetConfig();
    }
}