namespace OWML.Common.Menus
{
	public interface IModSelectorInput : IModInput<string>
	{
		OptionsSelectorElement SelectorElement { get; }

		int SelectedIndex { get; set; }

		void Initialize(string option, string[] options);

		IModSelectorInput Copy();

		IModSelectorInput Copy(string title);
	}
}
