using System;
using OWML.Common;
using OWML.Common.Menus;
using OWML.Utils;
using UnityEngine;

namespace OWML.ModHelper.Menus
{
	public class ModMenus : IModMenus
	{
		public IModMainMenu MainMenu { get; }

		public IModPauseMenu PauseMenu { get; }

		public IModsMenu ModsMenu { get; }

		public IModPopupManager PopupManager { get; }

		private readonly IModConsole _console;

		public ModMenus(
			IModEvents events,
			IModMainMenu mainMenu,
			IModPauseMenu pauseMenu,
			IModsMenu modsMenu,
			IModPopupManager popupManager,
			IModConsole console)
		{
			_console = console;
			MainMenu = mainMenu;
			PauseMenu = pauseMenu;
			ModsMenu = modsMenu;
			PopupManager = popupManager;

			events.Subscribe<PauseMenuManager>(Events.AfterStart);
			events.Subscribe<TitleScreenManager>(Events.AfterStart);
			events.Event += OnEvent;
		}

		private void OnEvent(MonoBehaviour behaviour, Events ev)
		{
			if (behaviour is PauseMenuManager pauseMenuManager &&
				ev == Events.AfterStart &&
				pauseMenuManager.name == "PauseMenuManagers")
			{
				InitPauseMenu(pauseMenuManager);
			}
			else if (behaviour is TitleScreenManager titleScreenManager &&
					 ev == Events.AfterStart)
			{
				InitMainMenu(titleScreenManager);
			}
		}

		private void InitMainMenu(TitleScreenManager titleScreenManager)
		{
			try
			{
				MainMenu.Initialize(titleScreenManager);
				var inputMenu = titleScreenManager.GetComponent<ProfileMenuManager>().GetValue<PopupInputMenu>("_createProfileConfirmPopup");
				PopupManager.Initialize(inputMenu, MainMenu.OptionsMenu);
				//ModsMenu.Initialize(this, MainMenu);
			}
			catch (Exception ex)
			{
				_console.WriteLine($"Menu system crashed: {ex}", MessageType.Error);
			}
		}

		private void InitPauseMenu(PauseMenuManager pauseMenuManager)
		{
			try
			{
				PauseMenu.Initialize(pauseMenuManager);
				//ModsMenu.Initialize(this, PauseMenu);
			}
			catch (Exception ex)
			{
				_console.WriteLine($"Menu system crashed: {ex}", MessageType.Error);
			}
		}
	}
}
