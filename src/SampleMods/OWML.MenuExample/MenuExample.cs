using OWML.Common;
using OWML.Common.Interfaces.Menus;
using OWML.ModHelper;
using System.Globalization;
using OWML.Common.Enums;
using OWML.ModHelper.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OWML.MenuExample
{
	public class MenuExample : ModBehaviour
	{
		public void Start()
		{
			gameObject.AddComponent<DebugGUI>();

			ModHelper.MenuHelper.PopupMenuManager.RegisterStartupPopup("Test Startup Popup");

			/*rebindOne = ModHelper.RebindingHelper.RegisterRebindable("Test Single 0.4", "Test Tooltip", "<Keyboard>/c", "<Gamepad>/leftShoulder");
			rebindTwo = ModHelper.RebindingHelper.RegisterRebindable("Test Dual", "Test Tooltip 2", "<Keyboard>/c", "<Gamepad>/rightShoulder", "<Keyboard>/v", "<Gamepad>/rightTrigger");
			rebindThree = ModHelper.RebindingHelper.RegisterRebindable("Test Single 0.1", "Test Tooltip 3", "<Keyboard>/y", "<Gamepad>/leftTrigger", 0.1f);

			rebindX = ModHelper.RebindingHelper.RegisterRebindable("Test X (Dual)", "Test Tooltip X", "<Keyboard>/a", "<Gamepad>/leftStick/left", "<Keyboard>/d", "<Gamepad>/leftStick/right");
			rebindY = ModHelper.RebindingHelper.RegisterRebindable("Test Y (Dual)", "Test Tooltip Y", "<Keyboard>/w", "<Gamepad>/leftStick/up", "<Keyboard>/s", "<Gamepad>/leftStick/down");
			rebindComp = ModHelper.RebindingHelper.RegisterComposite("Test Composite", rebindY, rebindX);*/

			rebindXButton = ModHelper.RebindingHelper.RegisterRebindable("Test X (Button)", "", Key.A, GamepadBinding.LeftStickLeft, Key.D, GamepadBinding.LeftStickRight, false);
			rebindYButton = ModHelper.RebindingHelper.RegisterRebindable("Test Y (Button)", "", Key.W, GamepadBinding.LeftStickUp, Key.S, GamepadBinding.LeftStickDown, false);
			rebindCompButton = ModHelper.RebindingHelper.RegisterComposite("Test Composite Button", rebindYButton, rebindXButton);

			rebindXAxis = ModHelper.RebindingHelper.RegisterRebindable("Test X (Axis)", "", Key.A, GamepadBinding.LeftStickLeft, Key.D, GamepadBinding.LeftStickRight, true);
			rebindYAxis = ModHelper.RebindingHelper.RegisterRebindable("Test Y (Axis)", "", Key.W, GamepadBinding.LeftStickUp, Key.S, GamepadBinding.LeftStickDown, true);
			rebindCompAxis = ModHelper.RebindingHelper.RegisterComposite("Test Composite Axis", rebindYAxis, rebindXAxis);

			rebindSingleButton = ModHelper.RebindingHelper.RegisterRebindable("Test (Button)", "", Key.Z, GamepadBinding.LeftTrigger, false);
			rebindSingleButton07Threshold = ModHelper.RebindingHelper.RegisterRebindable("Test (Button) 0.7", "", Key.Z, GamepadBinding.LeftTrigger, false, 0.7f);
			rebindSingleAxis = ModHelper.RebindingHelper.RegisterRebindable("Test (Axis)", "", Key.Z, GamepadBinding.LeftTrigger, true);
			rebindSingleAxis07Threshold = ModHelper.RebindingHelper.RegisterRebindable("Test (Axis) 0.7", "", Key.Z, GamepadBinding.LeftTrigger, true, 0.7f);
		}

		public IOWMLFourChoicePopupMenu FourChoicePopupMenu;
		public IOWMLPopupInputMenu PopupInput;

		/*public InputConsts.InputCommandType rebindOne;
		public InputConsts.InputCommandType rebindTwo;
		public InputConsts.InputCommandType rebindThree;
		public InputConsts.InputCommandType rebindX;
		public InputConsts.InputCommandType rebindY;
		public InputConsts.InputCommandType rebindComp;*/

		public InputConsts.InputCommandType rebindXButton;
		public InputConsts.InputCommandType rebindYButton;
		public InputConsts.InputCommandType rebindXAxis;
		public InputConsts.InputCommandType rebindYAxis;
		public InputConsts.InputCommandType rebindCompButton;
		public InputConsts.InputCommandType rebindCompAxis;
		public InputConsts.InputCommandType rebindSingleButton;
		public InputConsts.InputCommandType rebindSingleButton07Threshold;
		public InputConsts.InputCommandType rebindSingleAxis;
		public InputConsts.InputCommandType rebindSingleAxis07Threshold;

		public override void SetupTitleMenu(ITitleMenuManager titleManager)
		{
			var infoButton = titleManager.CreateTitleButton("INFO POPUP");
			var infoPopup = ModHelper.MenuHelper.PopupMenuManager.CreateInfoPopup("test info popup", "yarp");
			infoButton.OnSubmitAction += () => infoPopup.EnableMenu(true);
			infoPopup.OnActivateMenu += () => ModHelper.Console.WriteLine("info popup activate");
			infoPopup.OnDeactivateMenu += () => ModHelper.Console.WriteLine("info popup deactivate");

			var twoChoiceButton = titleManager.CreateTitleButton("TWO CHOICE");
			var twoChoicePopup = ModHelper.MenuHelper.PopupMenuManager.CreateTwoChoicePopup("test two choice popup", "oak", "narp");
			twoChoiceButton.OnSubmitAction += () => twoChoicePopup.EnableMenu(true);
			twoChoicePopup.OnActivateMenu += () => ModHelper.Console.WriteLine("two popup activate");
			twoChoicePopup.OnDeactivateMenu += () => ModHelper.Console.WriteLine("two popup deactivate");

			var threeChoiceButton = titleManager.CreateTitleButton("THREE CHOICE");
			var threeChoicePopup = ModHelper.MenuHelper.PopupMenuManager.CreateThreeChoicePopup("test three choice popup", "oak", "oak (better)", "narp");
			threeChoiceButton.OnSubmitAction += () => threeChoicePopup.EnableMenu(true);
			threeChoicePopup.OnPopupConfirm1 += () => ModHelper.Console.WriteLine("Confirm 1");
			threeChoicePopup.OnPopupConfirm2 += () => ModHelper.Console.WriteLine("Confirm 2");
			threeChoicePopup.OnActivateMenu += () => ModHelper.Console.WriteLine("three popup activate");
			threeChoicePopup.OnDeactivateMenu += () => ModHelper.Console.WriteLine("three popup deactivate");

			var fourChoiceButton = titleManager.CreateTitleButton("FOUR CHOICE");
			FourChoicePopupMenu = ModHelper.MenuHelper.PopupMenuManager.CreateFourChoicePopup("test four choice popup", "oak", "oak (better)", "oak (worse)", "narp");
			fourChoiceButton.OnSubmitAction += () => FourChoicePopupMenu.EnableMenu(true);
			FourChoicePopupMenu.OnPopupConfirm1 += () => ModHelper.Console.WriteLine("Confirm 1");
			FourChoicePopupMenu.OnPopupConfirm2 += () => ModHelper.Console.WriteLine("Confirm 2");
			FourChoicePopupMenu.OnPopupConfirm3 += () => ModHelper.Console.WriteLine("Confirm 3");
			FourChoicePopupMenu.OnActivateMenu += () => ModHelper.Console.WriteLine("four popup activate");
			FourChoicePopupMenu.OnDeactivateMenu += () => ModHelper.Console.WriteLine("four popup deactivate");

			var textButton = titleManager.CreateTitleButton("INPUT POPUP TEST");
			PopupInput = ModHelper.MenuHelper.PopupMenuManager.CreateInputFieldPopup("test text popup", "type a funny thing!", "ok", "cancel");
			textButton.OnSubmitAction += () => PopupInput.EnableMenu(true);
			PopupInput.OnPopupConfirm += () =>
			{
				ModHelper.Console.WriteLine(PopupInput.GetInputText());
			};

			PopupInput.OnActivateMenu += () => ModHelper.Console.WriteLine("text popup activate");
			PopupInput.OnDeactivateMenu += () => ModHelper.Console.WriteLine("text popup deactivate");
		}

		public override void CleanupTitleMenu()
		{
			ModHelper.Console.WriteLine($"CLEANUP TITLE MENU");

			FourChoicePopupMenu = null;
			PopupInput = null;
		}

		public override void SetupPauseMenu(IPauseMenuManager pauseManager)
		{
			var listMenu = pauseManager.MakePauseListMenu("TEST");
			var button = pauseManager.MakeMenuOpenButton("TEST", listMenu, 1, true);

			var button1 = pauseManager.MakeSimpleButton("1", 0, true, listMenu);
			var button2 = pauseManager.MakeSimpleButton("2", 1, true, listMenu);
			var button3 = pauseManager.MakeSimpleButton("3", 2, true, listMenu);

			pauseManager.PauseMenuOpened += LogOpened;
			pauseManager.PauseMenuClosed += LogClosed;
		}

		public override void CleanupPauseMenu()
		{
			ModHelper.MenuHelper.PauseMenuManager.PauseMenuOpened -= LogOpened;
			ModHelper.MenuHelper.PauseMenuManager.PauseMenuClosed -= LogClosed;
		}

		private void LogOpened()
		{
			ModHelper.Console.WriteLine($"PAUSE MENU OPENED!", MessageType.Success);
		}

		private void LogClosed()
		{
			ModHelper.Console.WriteLine($"PAUSE MENU CLOSED!", MessageType.Success);
		}

		public override void SetupOptionsMenu(IOptionsMenuManager optionsManager)
		{
			var infoPopup = ModHelper.MenuHelper.PopupMenuManager.CreateInfoPopup("test info popup", "yarp");
			var twoChoicePopup = ModHelper.MenuHelper.PopupMenuManager.CreateTwoChoicePopup("test two choice popup", "oak", "narp");
			var threeChoicePopup = ModHelper.MenuHelper.PopupMenuManager.CreateThreeChoicePopup("test three choice popup", "oak", "oak (better)", "narp");

			var (tabMenu, tabButton) = optionsManager.CreateTabWithSubTabs("TEST");
			var (subTab1Menu, subTab1Button) = optionsManager.AddSubTab(tabMenu, "TAB 1");
			var (subTab2Menu, subTab2Button) = optionsManager.AddSubTab(tabMenu, "TAB 2");

			var infoPopupButton = optionsManager.CreateButton(subTab1Menu, "Info Popup", "Opens an info popup.", MenuSide.LEFT);
			infoPopupButton.OnSubmitAction += () => infoPopup.EnableMenu(true);
			var twoButton = optionsManager.CreateButton(subTab1Menu, "Two Choice Popup", "Opens a two choice popup.", MenuSide.CENTER);
			twoButton.OnSubmitAction += () => twoChoicePopup.EnableMenu(true);
			var threeButton = optionsManager.CreateButton(subTab1Menu, "Three Choice Popup", "Opens a three choice popup.", MenuSide.RIGHT);
			threeButton.OnSubmitAction += () => threeChoicePopup.EnableMenu(true);

			optionsManager.AddSeparator(subTab2Menu, true);
			optionsManager.CreateLabel(subTab2Menu, "Test Left Label", MenuSide.LEFT);
			optionsManager.CreateLabel(subTab2Menu, "Test Center Label", MenuSide.CENTER);
			optionsManager.CreateLabel(subTab2Menu, "Test Right Label", MenuSide.RIGHT);
			var checkbox = optionsManager.AddCheckboxInput(subTab2Menu, "Test Checkbox", "* It's a test checkbox.", false);
			var toggle = optionsManager.AddToggleInput(subTab2Menu, "Test Toggle", "Option 1", "Option 2", "* It's a test toggle.", false);
			var selector = optionsManager.AddSelectorInput(subTab2Menu, "Test Selector", new[] { "Option 1", "Option 2", "Option 3" }, "* It's a test selector.", true, 0);
			var slider = optionsManager.AddSliderInput(subTab2Menu, "Test Slider", 0, 100, "* It's a test slider.", 50);
			var textInput = optionsManager.AddTextEntryInput(subTab2Menu, "Test Text", "Test", "* It's a test text input.", false);
			var numberInput = optionsManager.AddTextEntryInput(subTab2Menu, "Test Number", 0.ToString(CultureInfo.CurrentCulture), "* It's a test number input.", true);
		}

		public override void CleanupOptionsMenu()
		{
			ModHelper.Console.WriteLine($"CLEANUP OPTIONS MENU");
		}

		private void Print(IInputCommands c, string str)
		{
			var val = OWInput.GetValue(c);
			var axisVal = OWInput.GetAxisValue(c);

			if (OWInput.IsPressed(c))
			{
				ModHelper.Console.WriteLine($"{str} - {val} ||| {axisVal}");
			}

			if (OWInput.IsNewlyPressed(c))
			{
				ModHelper.Console.WriteLine($" - PRESSED");
			}

			if (OWInput.IsNewlyReleased(c))
			{
				ModHelper.Console.WriteLine($" - RELEASED");
			}
		}

		public void Update()
		{
			/*var c1 = InputLibrary.GetInputCommand(rebindOne);
			var c2 = InputLibrary.GetInputCommand(rebindTwo);
			var c3 = InputLibrary.GetInputCommand(rebindThree);
			var cX = InputLibrary.GetInputCommand(rebindX);
			var cY = InputLibrary.GetInputCommand(rebindY);
			var cComp = InputLibrary.GetInputCommand(rebindComp);

			c1.PressedThreshold = Mathf.Epsilon;
			c2.PressedThreshold = Mathf.Epsilon;
			c3.PressedThreshold = Mathf.Epsilon;

			Print(c1, "Test Single 0.4");
			Print(c2, "Test Dual");
			Print(c3, "Test Single 0.1");
			Print(cX, "Test X (Dual)");
			Print(cY, "Test Y (Dual)");
			Print(cComp, "Composite");*/

			if (FourChoicePopupMenu != null)
			{
				var rnd = new System.Random();
				FourChoicePopupMenu.SetText("blah", rnd.Next().ToString(), rnd.Next().ToString(), rnd.Next().ToString(), rnd.Next().ToString());
			}

			if (PopupInput != null)
			{
				var rnd = new System.Random();
				PopupInput.SetText("blah", rnd.Next().ToString(), rnd.Next().ToString(), rnd.Next().ToString());
			}
		}
	}
}
