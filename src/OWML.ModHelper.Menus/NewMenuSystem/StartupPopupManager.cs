using OWML.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWML.ModHelper.Menus.NewMenuSystem
{
	public class StartupPopupManager : IStartupPopupManager
	{
		private readonly IModConsole _console;

		internal static int ActivePopup;
		internal static List<int> PopupsToShow = new();
		internal static List<string> Popups = new List<string>()
		{
			"BASEGAME0",
			"BASEGAME1",
			"BASEGAME2"
		};

		public StartupPopupManager(IModConsole console, IHarmonyHelper harmony)
		{
			_console = console;

			harmony.AddPrefix<TitleScreenManager>(nameof(TitleScreenManager.DetermineStartupPopups), typeof(StartupPopupPatches), nameof(StartupPopupPatches.DetermineStartupPopups));
			harmony.AddPrefix<TitleScreenManager>(nameof(TitleScreenManager.OnUserConfirmStartupPopup), typeof(StartupPopupPatches), nameof(StartupPopupPatches.OnUserConfirmStartupPopup));
			harmony.AddPrefix<TitleScreenManager>(nameof(TitleScreenManager.TryShowStartupPopups), typeof(StartupPopupPatches), nameof(StartupPopupPatches.TryShowStartupPopups));
			harmony.AddPrefix<TitleScreenManager>(nameof(TitleScreenManager.TryShowStartupPopupsAndShowMenu), typeof(StartupPopupPatches), nameof(StartupPopupPatches.TryShowStartupPopupsAndShowMenu));
		}

		public void RegisterStartupPopup(string message)
		{
			PopupsToShow.Add(Popups.Count);
			Popups.Add(message);
		}
	}

	public static class StartupPopupPatches
	{
		public static bool DetermineStartupPopups(TitleScreenManager __instance)
		{
			if (__instance._profileManager.currentProfileGameSave.version == "NONE")
			{
				StartupPopupManager.PopupsToShow.Add(0);
			}

			var flag = EntitlementsManager.IsDlcOwned() == EntitlementsManager.AsyncOwnershipStatus.Owned;
			if (flag && (__instance._profileManager.currentProfileGameSave.shownPopups & StartupPopups.ReducedFrights) == StartupPopups.None)
			{
				StartupPopupManager.PopupsToShow.Add(1);
			}

			if (flag && (__instance._profileManager.currentProfileGameSave.shownPopups & StartupPopups.NewExhibit) == StartupPopups.None)
			{
				StartupPopupManager.PopupsToShow.Add(2);
			}

			return false;
		}

		public static bool TryShowStartupPopupsAndShowMenu(TitleScreenManager __instance)
		{
			if (StartupPopupManager.PopupsToShow.Count != 0)
			{
				__instance.TryShowStartupPopups();
				return false;
			}

			__instance._okCancelPopup.ResetPopup();
			__instance.SetUpMainMenu();

			if (__instance._autoResumeExpedition)
			{
				return false;
			}

			__instance.FadeInMenuOptions();

			return false;
		}

		public static bool TryShowStartupPopups(TitleScreenManager __instance)
		{
			string text = "AAAAGGGGGHH";

			StartupPopupManager.ActivePopup = StartupPopupManager.PopupsToShow.First();

			if (StartupPopupManager.ActivePopup <= 2)
			{
				switch(StartupPopupManager.ActivePopup)
				{
					case 0:
						text = UITextLibrary.GetString(UITextType.MenuMessage_InputUpdate);
						break;
					case 1:
						text = UITextLibrary.GetString(UITextType.MenuMessage_ReducedFrightOptionAvail);
						break;
					case 2:
						text = UITextLibrary.GetString(UITextType.MenuMessage_NewExhibit);
						break;
				}
			}
			else
			{
				text = StartupPopupManager.Popups[StartupPopupManager.ActivePopup];
			}

			__instance._inputModule.EnableInputs();
			__instance._titleMenuRaycastBlocker.blocksRaycasts = false;

			if (!__instance.MainMenuIsActive())
			{
				__instance._showPopupsOnReturnToMainMenu = true;
				return false;
			}

			__instance._okCancelPopup.ResetPopup();
			__instance._okCancelPopup.SetUpPopup(text, InputLibrary.menuConfirm, null, __instance._continuePrompt, null, true, false);
			__instance._okCancelPopup.OnPopupConfirm += __instance.OnUserConfirmStartupPopup;
			__instance._okCancelPopup.EnableMenu(true);

			return false;
		}

		public static bool OnUserConfirmStartupPopup(TitleScreenManager __instance)
		{
			StartupPopupManager.PopupsToShow.Remove(StartupPopupManager.ActivePopup);

			if (StartupPopupManager.ActivePopup <= 2)
			{
				switch (StartupPopupManager.ActivePopup)
				{
					case 0:
						PlayerData.SetShownPopups(StartupPopups.ResetInputs);
						break;
					case 1:
						PlayerData.SetShownPopups(StartupPopups.ReducedFrights);
						break;
					case 2:
						PlayerData.SetShownPopups(StartupPopups.NewExhibit);
						break;
				}

				PlayerData.SaveCurrentGame();
			}

			StartupPopupManager.ActivePopup = -1;
			__instance._okCancelPopup.OnPopupConfirm -= __instance.OnUserConfirmStartupPopup;
			__instance._inputModule.DisableInputs();
			__instance._titleMenuRaycastBlocker.blocksRaycasts = true;
			__instance.TryShowStartupPopupsAndShowMenu();

			return false;
		}
	}
}
