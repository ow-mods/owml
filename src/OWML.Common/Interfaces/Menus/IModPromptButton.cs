namespace OWML.Common.Interfaces.Menus
{
    public interface IModPromptButton : IModButton
    {
        string DefaultTitle { get; }
        ScreenPrompt Prompt { get; set; }
    }
}