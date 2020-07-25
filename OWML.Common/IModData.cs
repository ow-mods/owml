namespace OWML.Common
{
    public interface IModData
    {
        IModManifest Manifest { get; }
        IModConfig Config { get; }
        IModConfig DefaultConfig { get; }
        bool RequireVR { get; }
        bool RequireReload { get; }
        void UpdateSnapshot();
    }
}