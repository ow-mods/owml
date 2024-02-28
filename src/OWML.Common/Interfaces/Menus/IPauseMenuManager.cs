namespace OWML.Common
{
	public interface IPauseMenuManager
	{
		public Menu MakePauseListMenu(string title);

		public SubmitAction MakeSimpleButton(string name, int index, bool fromTop, Menu customMenu = null);

		public SubmitAction MakeMenuOpenButton(string name, Menu menuToOpen, int index, bool fromTop, Menu customMenu = null);

		public void SetButtonText(SubmitAction button, string text);

		public void SetButtonIndex(SubmitAction button, int index, bool fromTop);
	}
}
