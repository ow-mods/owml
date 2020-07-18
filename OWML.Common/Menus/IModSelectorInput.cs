namespace OWML.Common.Menus
{
    public interface IModSelectorInput : IModInput<int>
    {
        void Initialize(int index, string[] options);
        IModSelectorInput Copy();
        IModSelectorInput Copy(string title);
    }
}
