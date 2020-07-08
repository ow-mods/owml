namespace OWML.Common.Menus
{
    public interface IModMainMenu : IModMenu
    {
        IModTabbedMenu OptionsMenu { get; }

        IModTitleButton ResumeExpeditionButton { get; }
        IModTitleButton NewExpeditionButton { get; }
        IModTitleButton OptionsButton { get; }
        IModTitleButton ViewCreditsButton { get; }
        IModTitleButton SwitchProfileButton { get; }
        IModTitleButton QuitButton { get; }

        void Initialize(TitleScreenManager titleScreenManager);
    }
}
