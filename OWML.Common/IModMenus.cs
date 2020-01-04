namespace OWML.Common
{
    public interface IModMenus
    {
        IModMenu MainMenu { get; }
        IModMenu PauseMenu { get; }
        IModMenu OptionsMenu { get; }
    }
}
