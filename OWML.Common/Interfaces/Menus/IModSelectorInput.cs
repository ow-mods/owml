namespace OWML.Common.Interfaces.Menus
{
    public interface IModSelectorInput : IModInput<string>
    {
        int SelectedIndex { get; set; }
        void Initialize(string option, string[] options);
        IModSelectorInput Copy();
        IModSelectorInput Copy(string title);
    }
}
