namespace OWML.Common.Menus
{
	public interface IModPromptButton : IModButton
	{
		string DefaultTitle { get; }

		ScreenPrompt Prompt { get; set; }
	}
}