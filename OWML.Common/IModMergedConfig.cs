namespace OWML.Common
{
    public interface IModMergedConfig : IModConfig
    {
        void SaveToStorage();
        void Reset();
        IModConfig Copy();
    }
}
