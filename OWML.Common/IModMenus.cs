namespace OWML.Common
{
    public interface IModMenus
    {
        IModMenu MainMenu { get; }
        IModPopupMenu PauseMenu { get; }
        IModTabbedMenu OptionsMenu { get; }
    }
}
