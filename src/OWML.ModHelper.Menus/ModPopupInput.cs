﻿using System.Collections.Generic;
using OWML.Common.Menus;
using OWML.ModHelper.Input;
using OWML.Utils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace OWML.ModHelper.Menus
{
	public abstract class ModPopupInput<T> : ModInput<T>
	{
		public override bool IsSelected => ToggleElement.GetValue<bool>("_amISelected");

		protected readonly TwoButtonToggleElement ToggleElement;
		protected ModCommandListener CommandListener;

		private readonly List<IInputCommands> _openCommands = new()
		{
			InputLibrary.menuConfirm,
			InputLibrary.enter,
			InputLibrary.enter2
		};

		protected ModPopupInput(TwoButtonToggleElement toggle, IModMenu menu)
			: base(toggle, menu)
		{
			ToggleElement = toggle;

			var noButton = ToggleElement.GetValue<Button>("_buttonFalse");
			noButton.transform.parent.gameObject.SetActive(false);

			var buttonParent = toggle.GetValue<Button>("_buttonTrue").transform.parent;
			var layoutGroup = buttonParent.parent.GetComponent<HorizontalLayoutGroup>();
			layoutGroup.childControlWidth = true;
			layoutGroup.childForceExpandWidth = true;

			buttonParent.GetComponent<LayoutElement>().preferredWidth = 100;

			SetupCommands();
		}

		private void SetupCommands()
		{
			var listenerObject = new GameObject();
			CommandListener = listenerObject.AddComponent<ModCommandListener>();
			_openCommands.ForEach(CommandListener.AddToListener);
			CommandListener.OnNewlyPressed += OnOpenCommand;
		}

		protected void Subscribe(IModButtonBase button)
		{
			button.OnClick += Open;
		}

		protected virtual void OnOpenCommand(IInputCommands command)
		{
			if (IsSelected && _openCommands.Contains(command))
			{
				command.ConsumeInput();
				Open();
			}
		}

		protected virtual void Open()
		{
			EventSystem.current.SetSelectedGameObject(ToggleElement.gameObject); // make sure it gets selected after popup closes
		}
	}
}
