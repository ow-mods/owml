namespace OWML.Common
{
    public interface IModMenus
    {
        IModMenu MainMenu { get; }
        IModPopupMenu PauseMenu { get; }
        IModPopupMenu OptionsMenu { get; }
        IModPopupMenu InputMenu { get; }
    }
}
