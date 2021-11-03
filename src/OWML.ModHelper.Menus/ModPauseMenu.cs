using OWML.Common;
using OWML.Common.Menus;
using OWML.Utils;

namespace OWML.ModHelper.Menus
{
	public class ModPauseMenu : ModPopupMenu, IModPauseMenu
	{
		public IModTabbedMenu OptionsMenu { get; }

		public IModButton ResumeButton { get; private set; }

		public IModButton OptionsButton { get; private set; }

		public IModButton QuitButton { get; private set; }

		public ModPauseMenu(IModTabbedMenu optionsMenu, IModConsole console)
			: base(console) =>
			OptionsMenu = optionsMenu;

		public void Initialize(PauseMenuManager pauseMenuManager)
		{
			var optionsMenu = pauseMenuManager.transform.parent.GetComponentInChildren<TabbedMenu>(true);
			OptionsMenu.Initialize(optionsMenu, 1);

			var pauseMenu = pauseMenuManager.GetValue<Menu>("_pauseMenu");
			base.Initialize(pauseMenu);

			ResumeButton = GetTitleButton("Button-Unpause");
			OptionsButton = GetTitleButton("Button-Options");
			QuitButton = GetTitleButton("Button-ExitToMainMenu");

			InvokeOnInit();
		}
	}
}
