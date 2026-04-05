using System.Collections.Generic;
using OWML.Common;
using OWML.ModHelper;
using UnityEngine.InputSystem;
using UnityEngine;
using OWML.Common.Interfaces.Menus;

namespace OWML.MenuExample
{
	public class MenuExample : ModBehaviour
	{
		public void Start()
		{
			ModHelper.MenuHelper.PopupMenuManager.RegisterStartupPopup("Test Startup Popup");
		}

		public IOWMLFourChoicePopupMenu FourChoicePopupMenu;
		public IOWMLPopupInputMenu PopupInput;

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

			var checkbox = optionsManager.AddCheckboxInput(subTab2Menu, "Test Checkbox", "* It's a test checkbox.", false);
			var toggle = optionsManager.AddToggleInput(subTab2Menu, "Test Toggle", "Option 1", "Option 2", "* It's a test toggle.", false);
			var selector = optionsManager.AddSelectorInput(subTab2Menu, "Test Selector", new[] { "Option 1", "Option 2", "Option 3" }, "* It's a test selector.", true, 0);
			var slider = optionsManager.AddSliderInput(subTab2Menu, "Test Slider", 0, 100, "* It's a test slider.", 50);
		}

		public override void CleanupOptionsMenu()
		{
			ModHelper.Console.WriteLine($"CLEANUP OPTIONS MENU");
		}

		public void Update()
		{
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
