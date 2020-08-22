namespace OWML.Common
{
    public interface IModData
    {
        IModManifest Manifest { get; }
        IModMergedConfig Config { get; }
        bool Enabled { get; }
        bool RequireVR { get; }
        bool RequireReload { get; }

        void UpdateSnapshot();
    }
}