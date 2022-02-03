﻿using System;
using System.Collections.Generic;
using System.Linq;
using OWML.Utils;
using OWML.Common;
using OWML.Common.Menus;
using OWML.ModHelper.Input;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
	public class ModMenuWithSelectables : ModPopupMenu, IModMenuWithSelectables
	{
		public event Action OnCancel;

		protected List<Selectable> Selectables;
		protected ModCommandListener CommandListener;

		public ModMenuWithSelectables(IModConsole console)
			: base(console)
		{
		}

		private void SetupCommands()
		{
			var listenerObject = new GameObject("ConfigurationMenu_Listener");
			CommandListener = listenerObject.AddComponent<ModCommandListener>();
			CommandListener.AddToListener(InputLibrary.confirm);
			CommandListener.AddToListener(InputLibrary.enter2); // keypad's Enter
			CommandListener.AddToListener(InputLibrary.cancel);
			CommandListener.AddToListener(InputLibrary.escape);
			CommandListener.AddToListener(InputLibrary.setDefaults);
		}

		protected virtual void SetupButtons(Menu menu)
		{
			var promptButtons = GetParentPromptButtons(menu);
			var saveButton = promptButtons.FirstOrDefault(x => x.Button.name == "UIElement-SaveAndExit");
			var resetButton = promptButtons.FirstOrDefault(x => x.Button.name == "UIElement-ResetToDefaultsButton");

			if (saveButton == null || resetButton == null/* || cancelButton == null*/)
			{
				Console.WriteLine("Failed to setup menu with selectables.", MessageType.Error);
				return;
			}

			saveButton.OnClick += OnSave;
			resetButton.OnClick += OnReset;

			saveButton.Prompt = new ScreenPrompt(InputLibrary.confirm, saveButton.DefaultTitle);
			resetButton.Prompt = new ScreenPrompt(InputLibrary.setDefaults, resetButton.DefaultTitle);
		}

		private IList<ModPromptButton> GetParentPromptButtons(Menu menu)
		{
			var parent = menu.transform.parent;

			if (parent.Find("OptionsButtons") == null)
			{
				parent = parent.parent;
			}

			return parent
				.GetComponentsInChildren<ButtonWithHotkeyImageElement>(true)
				.Select(x => x.GetComponent<Button>())
				.Select(x => new ModPromptButton(x, this, Console))
				.ToList();
		}
			

		public override void Initialize(Menu menu)
		{
			var layoutGroup = menu.GetComponentsInChildren<VerticalLayoutGroup>(true)
				.Single(x => x.name == "Content");
			Initialize(menu, layoutGroup);

			if (CommandListener == null)
			{
				SetupCommands();
			}
			SetupButtons(menu);

			foreach (Transform child in layoutGroup.transform)
			{
				child.gameObject.SetActive(false);
			}
			Menu.OnActivateMenu += OnActivateMenu;
			Menu.OnDeactivateMenu += OnDeactivateMenu;
			UpdateNavigation();
		}

		public override void SelectFirst() => Menu.SetSelectOnActivate(Selectables[0]);

		public override void UpdateNavigation()
		{
			var options = Layout.GetComponentsInChildren<MenuOption>();
			Menu.SetValue("_menuOptions", options.ToArray());
			Selectables = Layout.GetComponentsInChildren<MenuOption>()
				.Select(x => x.GetComponent<Selectable>())
				.Where(x => x != null).ToList();
			UpdateNavigation(Selectables);
		}

		protected virtual void AddSelectable(Selectable selectable, int index)
		{
			Selectables.Insert(index, selectable);

			var current = Selectables[index];
			var next = Selectables[(index + 1) % Selectables.Count];
			var previous = Selectables[(index - 1 + Selectables.Count) % Selectables.Count];

			var navigation = next.navigation;
			navigation.selectOnUp = current;
			next.navigation = navigation;

			navigation = previous.navigation;
			navigation.selectOnDown = current;
			previous.navigation = navigation;

			navigation = current.navigation;
			navigation.selectOnDown = next;
			navigation.selectOnUp = previous;
			current.navigation = navigation;
		}

		protected virtual void AddSelectable(Selectable selectable)
		{
			AddSelectable(selectable, Selectables.Count);
		}

		protected virtual void OnActivateMenu()
		{
			CommandListener.OnNewlyPressed += OnButton;
		}

		protected virtual void OnDeactivateMenu()
		{
			CommandListener.OnNewlyPressed -= OnButton;
		}

		protected virtual void OnButton(IInputCommands command)
		{
			if (command == InputLibrary.confirm || command == InputLibrary.enter2 || command == InputLibrary.cancel || command == InputLibrary.escape)
			{
				command.ConsumeInput();
				OnSave();
			}
			if (command == InputLibrary.setDefaults)
			{
				command.ConsumeInput();
				OnReset();
			}
		}

		protected virtual void OnSave()
		{
			Close();
		}

		protected virtual void OnExit()
		{
			OnCancel?.Invoke();
			Close();
		}

		protected virtual void OnReset() { }
	}
}
