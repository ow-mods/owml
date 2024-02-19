using OWML.Common;
using OWML.Common.Interfaces.Menus;
using OWML.ModHelper.Menus.CustomInputs;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OWML.ModHelper.Menus.NewMenuSystem
{
	public class PopupMenuManager : IPopupMenuManager
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

		private GameObject _twoChoicePopupBase;
		private GameObject _inputPopupBase;

		public PopupMenuManager(IModConsole console, IHarmonyHelper harmony)
		{
			_console = console;

			LoadManager.OnCompleteSceneLoad += LoadManager_OnCompleteSceneLoad;

			harmony.AddPrefix<TitleScreenManager>(nameof(TitleScreenManager.DetermineStartupPopups), typeof(StartupPopupPatches), nameof(StartupPopupPatches.DetermineStartupPopups));
			harmony.AddPrefix<TitleScreenManager>(nameof(TitleScreenManager.OnUserConfirmStartupPopup), typeof(StartupPopupPatches), nameof(StartupPopupPatches.OnUserConfirmStartupPopup));
			harmony.AddPrefix<TitleScreenManager>(nameof(TitleScreenManager.TryShowStartupPopups), typeof(StartupPopupPatches), nameof(StartupPopupPatches.TryShowStartupPopups));
			harmony.AddPrefix<TitleScreenManager>(nameof(TitleScreenManager.TryShowStartupPopupsAndShowMenu), typeof(StartupPopupPatches), nameof(StartupPopupPatches.TryShowStartupPopupsAndShowMenu));

			_inputPopupBase = Object.Instantiate(Resources.FindObjectsOfTypeAll<PopupMenu>().First(x => x.name == "InputField-Popup" && x.transform.parent.name == "PopupCanvas" && x.transform.parent.parent.name == "TitleMenu").gameObject);
			Object.DontDestroyOnLoad(_inputPopupBase);
			_inputPopupBase.SetActive(false);

			_twoChoicePopupBase = Object.Instantiate(Resources.FindObjectsOfTypeAll<PopupMenu>().First(x => x.name == "TwoButton-Popup" && x.transform.parent.name == "PopupCanvas" && x.transform.parent.parent.name == "TitleMenu").gameObject);
			Object.DontDestroyOnLoad(_twoChoicePopupBase);
			_twoChoicePopupBase.SetActive(false);
		}

		private void LoadManager_OnCompleteSceneLoad(OWScene originalScene, OWScene loadScene)
		{
			if (loadScene != OWScene.TitleScreen)
			{
				return;
			}
		}

		public void RegisterStartupPopup(string message)
		{
			PopupsToShow.Add(Popups.Count);
			Popups.Add(message);
		}

		public PopupMenu CreateTwoChoicePopup(string message, string confirmText, string cancelText)
		{
			var newPopup = Object.Instantiate(_twoChoicePopupBase);

			switch (LoadManager.GetCurrentScene())
			{
				case OWScene.TitleScreen:
					newPopup.transform.parent = GameObject.Find("/TitleMenu/PopupCanvas").transform;
					break;
				case OWScene.SolarSystem:
				case OWScene.EyeOfTheUniverse:
					newPopup.transform.parent = GameObject.Find("/PauseMenu/PopupCanvas").transform;
					break;
				default:
					_console.WriteLine($"Cannot create popup in scene {LoadManager.GetCurrentScene()} !", MessageType.Error);
					break;
			}

			newPopup.transform.localPosition = Vector3.zero;
			newPopup.transform.localScale = Vector3.one;
			newPopup.GetComponentsInChildren<LocalizedText>().ToList().ForEach(x => Object.Destroy(x));

			var popup = newPopup.GetComponent<PopupMenu>();
			popup.SetUpPopup(message, InputLibrary.menuConfirm, InputLibrary.cancel, new ScreenPrompt(confirmText), new ScreenPrompt(cancelText), true, true);
			return popup;
		}

		public PopupMenu CreateInfoPopup(string message, string continueButtonText)
		{
			var newPopup = Object.Instantiate(_twoChoicePopupBase);

			switch (LoadManager.GetCurrentScene())
			{
				case OWScene.TitleScreen:
					newPopup.transform.parent = GameObject.Find("/TitleMenu/PopupCanvas").transform;
					break;
				case OWScene.SolarSystem:
				case OWScene.EyeOfTheUniverse:
					newPopup.transform.parent = GameObject.Find("/PauseMenu/PopupCanvas").transform;
					break;
				default:
					_console.WriteLine($"Cannot create popup in scene {LoadManager.GetCurrentScene()} !", MessageType.Error);
					break;
			}

			newPopup.transform.localPosition = Vector3.zero;
			newPopup.transform.localScale = Vector3.one;
			newPopup.GetComponentsInChildren<LocalizedText>().ToList().ForEach(Object.Destroy);

			var popup = newPopup.GetComponent<PopupMenu>();
			popup.SetUpPopup(
				message,
				InputLibrary.menuConfirm,
				InputLibrary.cancel,
				new ScreenPrompt(continueButtonText),
				null,
				true,
				false);
			return popup;
		}

		public IOWMLThreeChoicePopupMenu CreateThreeChoicePopup(string message, string confirm1Text, string confirm2Text, string cancelText)
		{
			var newPopup = Object.Instantiate(_twoChoicePopupBase);

			switch (LoadManager.GetCurrentScene())
			{
				case OWScene.TitleScreen:
					newPopup.transform.parent = GameObject.Find("/TitleMenu/PopupCanvas").transform;
					break;
				case OWScene.SolarSystem:
				case OWScene.EyeOfTheUniverse:
					newPopup.transform.parent = GameObject.Find("/PauseMenu/PopupCanvas").transform;
					break;
				default:
					_console.WriteLine($"Cannot create popup in scene {LoadManager.GetCurrentScene()} !", MessageType.Error);
					break;
			}

			newPopup.transform.localPosition = Vector3.zero;
			newPopup.transform.localScale = Vector3.one;
			newPopup.GetComponentsInChildren<LocalizedText>().ToList().ForEach(Object.Destroy);

			var originalPopup = newPopup.GetComponent<PopupMenu>();

			var ok1Button = originalPopup._confirmButton.gameObject;

			var ok2Button = Object.Instantiate(ok1Button, ok1Button.transform.parent);
			ok2Button.transform.SetSiblingIndex(1);

			var popup = newPopup.AddComponent<OWMLThreeChoicePopupMenu>();
			popup._labelText = originalPopup._labelText;
			popup._cancelAction = originalPopup._cancelAction;
			popup._ok1Action = originalPopup._okAction;
			popup._ok2Action = ok2Button.GetComponent<SubmitAction>();
			popup._cancelButton = originalPopup._cancelButton;
			popup._confirmButton1 = originalPopup._confirmButton;
			popup._confirmButton2 = ok2Button.GetComponent<ButtonWithHotkeyImageElement>();
			popup._rootCanvas = originalPopup._rootCanvas;
			popup._menuActivationRoot = originalPopup._menuActivationRoot;
			popup._startEnabled = originalPopup._startEnabled;
			popup._selectOnActivate = originalPopup._selectOnActivate;
			popup._selectableItemsRoot = originalPopup._selectableItemsRoot;
			popup._subMenus = originalPopup._subMenus;
			popup._menuOptions = originalPopup._menuOptions;
			popup._addToMenuStackManager = true;
			popup.SetUpPopup(
				message,
				InputLibrary.menuConfirm,
				InputLibrary.confirm2,
				InputLibrary.cancel,
				new ScreenPrompt(confirm1Text),
				new ScreenPrompt(confirm2Text),
				new ScreenPrompt(cancelText));
			return popup;
		}

		public IOWMLPopupInputMenu CreateInputFieldPopup(string message, string placeholderMessage, string confirmText, string cancelText)
		{
			var newPopup = Object.Instantiate(_inputPopupBase);

			switch (LoadManager.GetCurrentScene())
			{
				case OWScene.TitleScreen:
					newPopup.transform.parent = GameObject.Find("/TitleMenu/PopupCanvas").transform;
					break;
				case OWScene.SolarSystem:
				case OWScene.EyeOfTheUniverse:
					newPopup.transform.parent = GameObject.Find("/PauseMenu/PopupCanvas").transform;
					break;
				default:
					_console.WriteLine($"Cannot create popup in scene {LoadManager.GetCurrentScene()} !", MessageType.Error);
					break;
			}

			newPopup.transform.localPosition = Vector3.zero;
			newPopup.transform.localScale = Vector3.one;
			newPopup.GetComponentsInChildren<LocalizedText>().ToList().ForEach(x => Object.Destroy(x));

			var oldpopup = newPopup.GetComponent<PopupInputMenu>();

			var popup = newPopup.AddComponent<OWMLPopupInputMenu>();
			popup._menuActivationRoot = oldpopup._menuActivationRoot;
			popup._startEnabled = false;
			popup._selectOnActivate = oldpopup._selectOnActivate;
			popup._selectableItemsRoot = oldpopup._selectableItemsRoot;
			popup._subMenus = oldpopup._subMenus;
			popup._menuOptions = oldpopup._menuOptions;
			popup._setMenuNavigationOnActivate = true;
			popup._addToMenuStackManager = true;
			popup._labelText = oldpopup._labelText;
			popup._cancelAction = oldpopup._cancelAction;
			popup._okAction = oldpopup._okAction;
			popup._cancelButton = oldpopup._cancelButton;
			popup._confirmButton = oldpopup._confirmButton;
			popup._rootCanvas = oldpopup._rootCanvas;
			popup._inputField = oldpopup._inputField;
			popup._inputFieldEventListener = oldpopup._inputFieldEventListener;

			Object.Destroy(oldpopup);

			var usingGamepad = OWInput.UsingGamepad();

			popup.OnActivateMenu += () =>
			{
				var confirmPrompt = new ScreenPrompt(InputLibrary.confirm, confirmText, 0, ScreenPrompt.DisplayState.Normal, false);

				var screenPrompt = new ScreenPrompt(InputLibrary.escape, cancelText, 0, ScreenPrompt.DisplayState.Normal, false);
				if (usingGamepad)
				{
					screenPrompt = new ScreenPrompt(InputLibrary.cancel, cancelText, 0, ScreenPrompt.DisplayState.Normal, false);
				}

				if (usingGamepad)
				{
					popup.SetUpPopup(message, InputLibrary.confirm, InputLibrary.cancel, confirmPrompt, screenPrompt, false, true);
				}
				else
				{
					popup.SetUpPopup(message, InputLibrary.confirm, InputLibrary.escape, confirmPrompt, screenPrompt, false, true);
				}

				popup.SetInputFieldPlaceholderText(placeholderMessage);
			};

			return popup;
		}

		public IOWMLFourChoicePopupMenu CreateFourChoicePopup(string message, string confirm1Text, string confirm2Text, string confirm3Text, string cancelText)
		{
			var newPopup = Object.Instantiate(_twoChoicePopupBase);

			switch (LoadManager.GetCurrentScene())
			{
				case OWScene.TitleScreen:
					newPopup.transform.parent = GameObject.Find("/TitleMenu/PopupCanvas").transform;
					break;
				case OWScene.SolarSystem:
				case OWScene.EyeOfTheUniverse:
					newPopup.transform.parent = GameObject.Find("/PauseMenu/PopupCanvas").transform;
					break;
				default:
					_console.WriteLine($"Cannot create popup in scene {LoadManager.GetCurrentScene()} !", MessageType.Error);
					break;
			}

			newPopup.transform.localPosition = Vector3.zero;
			newPopup.transform.localScale = Vector3.one;
			newPopup.GetComponentsInChildren<LocalizedText>().ToList().ForEach(Object.Destroy);

			var originalPopup = newPopup.GetComponent<PopupMenu>();

			var ok1Button = originalPopup._confirmButton.gameObject;

			var ok2Button = Object.Instantiate(ok1Button, ok1Button.transform.parent);
			ok2Button.transform.SetSiblingIndex(1);

			var ok3Button = Object.Instantiate(ok1Button, ok1Button.transform.parent);
			ok3Button.transform.SetSiblingIndex(2);

			var popup = newPopup.AddComponent<OWMLFourChoicePopupMenu>();
			popup._labelText = originalPopup._labelText;
			popup._cancelAction = originalPopup._cancelAction;
			popup._ok1Action = originalPopup._okAction;
			popup._ok2Action = ok2Button.GetComponent<SubmitAction>();
			popup._ok3Action = ok3Button.GetComponent<SubmitAction>();
			popup._cancelButton = originalPopup._cancelButton;
			popup._confirmButton1 = originalPopup._confirmButton;
			popup._confirmButton2 = ok2Button.GetComponent<ButtonWithHotkeyImageElement>();
			popup._confirmButton3 = ok3Button.GetComponent<ButtonWithHotkeyImageElement>();
			popup._rootCanvas = originalPopup._rootCanvas;
			popup._menuActivationRoot = originalPopup._menuActivationRoot;
			popup._startEnabled = originalPopup._startEnabled;
			popup._selectOnActivate = originalPopup._selectOnActivate;
			popup._selectableItemsRoot = originalPopup._selectableItemsRoot;
			popup._subMenus = originalPopup._subMenus;
			popup._menuOptions = originalPopup._menuOptions;
			popup._addToMenuStackManager = true;
			popup.SetUpPopup(
				message,
				InputLibrary.menuConfirm,
				InputLibrary.confirm2,
				InputLibrary.signalscope,
				InputLibrary.cancel,
				new ScreenPrompt(confirm1Text),
				new ScreenPrompt(confirm2Text),
				new ScreenPrompt(confirm3Text),
				new ScreenPrompt(cancelText));
			return popup;
		}
	}

	public static class StartupPopupPatches
	{
		public static bool DetermineStartupPopups(TitleScreenManager __instance)
		{
			if (__instance._profileManager.currentProfileGameSave.version == "NONE")
			{
				PopupMenuManager.PopupsToShow.Add(0);
			}

			var flag = EntitlementsManager.IsDlcOwned() == EntitlementsManager.AsyncOwnershipStatus.Owned;
			if (flag && (__instance._profileManager.currentProfileGameSave.shownPopups & StartupPopups.ReducedFrights) == StartupPopups.None)
			{
				PopupMenuManager.PopupsToShow.Add(1);
			}

			if (flag && (__instance._profileManager.currentProfileGameSave.shownPopups & StartupPopups.NewExhibit) == StartupPopups.None)
			{
				PopupMenuManager.PopupsToShow.Add(2);
			}

			return false;
		}

		public static bool TryShowStartupPopupsAndShowMenu(TitleScreenManager __instance, bool firstTimeRun)
		{
			if (__instance._setGammaMenuCallback)
			{
				PlayerData.SetRanFirstRunGammaSetup(true);
				__instance._setGammaMenuCallback = false;
				__instance._firstTimeGammaSetup.OnGammaMenuFadeOutComplete -= __instance.TryShowStartupPopupsAndShowMenu;
			}

			if (!PlayerData.RanFirstRunGammaSetup() && __instance.MainMenuIsActive())
			{
				__instance._inputModule.EnableInputs();
				__instance._setGammaMenuCallback = true;
				__instance._firstTimeGammaSetup.OnGammaMenuFadeOutComplete += __instance.TryShowStartupPopupsAndShowMenu;
				__instance._firstTimeGammaSetup.OnDeactivateMenu += __instance.OnGammaMenuDeactivate;
				__instance._firstTimeGammaSetup.ActivateAsFirstTimeSetup();
				return false;
			}

			if (PopupMenuManager.PopupsToShow.Count == 0)
			{
				__instance._okCancelPopup.ResetPopup();
				__instance.SetUpMainMenu();

				if (__instance._autoResumeExpedition)
				{
					return false;
				}

				if (firstTimeRun)
				{
					__instance.FadeInMenuOptions();
					return false;
				}
			}
			else
			{
				__instance.TryShowStartupPopups();
			}

			return false;
		}

		public static bool TryShowStartupPopups(TitleScreenManager __instance)
		{
			string text = "AAAAGGGGGHH";

			PopupMenuManager.ActivePopup = PopupMenuManager.PopupsToShow.First();

			if (PopupMenuManager.ActivePopup <= 2)
			{
				switch(PopupMenuManager.ActivePopup)
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
				text = PopupMenuManager.Popups[PopupMenuManager.ActivePopup];
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
			PopupMenuManager.PopupsToShow.Remove(PopupMenuManager.ActivePopup);

			if (PopupMenuManager.ActivePopup <= 2)
			{
				switch (PopupMenuManager.ActivePopup)
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

			PopupMenuManager.ActivePopup = -1;
			__instance._okCancelPopup.OnPopupConfirm -= __instance.OnUserConfirmStartupPopup;
			__instance._inputModule.DisableInputs();
			__instance._titleMenuRaycastBlocker.blocksRaycasts = true;
			__instance.TryShowStartupPopupsAndShowMenu(true);

			return false;
		}
	}
}
