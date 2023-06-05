namespace OWML.Common.Menus
{
	public interface IModTabbedMenu : IModPopupMenu
	{
		IModTabMenu GameplayTab { get; }

		IModTabMenu AudioTab { get; }

		IModTabMenu InputTab { get; }

		IModTabMenu GraphicsTab { get; }

		new IModTabbedMenu Copy();

		void Initialize(TabbedMenu menu, int menuStackCount);

		new TabbedMenu Menu { get; }

		void AddTab(IModTabMenu tab, bool enable = true);

		void SetIsBlocking(bool isBlocking);
	}
}
