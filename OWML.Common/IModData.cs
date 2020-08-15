namespace OWML.Common
{
    public interface IModData
    {
        IModManifest Manifest { get; }
        IModConfig Config { get; }
        bool Enabled { get; }
        bool RequireVR { get; }
        bool RequireReload { get; }
        IModConfig UserConfig { get; }

        void UpdateSnapshot();
        void ResetConfigToDefaults();
    }
}