namespace OWML.Common.Menus
{
	public interface IModTabMenu : IModPopupMenu
	{
		void Initialize(TabButton tabButton);

		TabButton TabButton { get; }

		void HideButton();

		new IModTabMenu Copy();

		new IModTabMenu Copy(string title);
	}
}