namespace OWML.Common
{
    public interface IModMergedConfig : IModConfig
    {
        void SaveToStorage();
        IModConfig Copy();
    }
}
