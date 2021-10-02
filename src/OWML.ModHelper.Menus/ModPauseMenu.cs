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
			//var pauseMenuManager = settingsManager.GetComponent<PauseMenuManager>();
			//var optionsMenu = pauseMenuManager.GetValue<TabbedMenu>("_mainSettingsMenu"); todo
			var optionsMenu = pauseMenuManager.transform.parent.GetComponentInChildren<TabbedMenu>();
			OptionsMenu.Initialize(optionsMenu);

			var pauseMenu = pauseMenuManager.GetValue<Menu>("_pauseMenu");
			base.Initialize(pauseMenu);

			ResumeButton = GetTitleButton("Button-Unpause");
			OptionsButton = GetTitleButton("Button-Options");
			QuitButton = GetTitleButton("Button-ExitToMainMenu");

			InvokeOnInit();
		}
	}
}
