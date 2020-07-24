namespace OWML.Common
{
    public interface IModData
    {
        IModManifest Manifest { get; }
        IModConfig Config { get; }
        IModConfig DefaultConfig { get; }
        bool Enabled { get; }
        bool RequireVR { get; }

        void ResetConfigToDefaults();
        void FixConfigs();
    }
}