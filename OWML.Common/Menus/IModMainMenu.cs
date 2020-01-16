namespace OWML.Common.Menus
{
    public interface IModMainMenu : IModMenu
    {
        IModTabbedMenu OptionsMenu { get; }

        IModButton ResumeExpeditionButton { get; }
        IModButton NewExpeditionButton { get; }
        IModButton OptionsButton { get; }
        IModButton ViewCreditsButton { get; }
        IModButton SwitchProfileButton { get; }
        IModButton QuitButton { get; }

        void Initialize(TitleScreenManager titleScreenManager);
    }
}
