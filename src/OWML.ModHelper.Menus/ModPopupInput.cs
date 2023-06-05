using System.Collections.Generic;
using OWML.Common.Menus;
using OWML.ModHelper.Input;
using OWML.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace OWML.ModHelper.Menus
{
	public abstract class ModPopupInput<T> : ModInput<T>
	{
		public override bool IsSelected => Element.GetValue<bool>("_amISelected");

		protected readonly OptionsSelectorElement SelectorElement;

		protected ModCommandListener CommandListener;

		private readonly List<IInputCommands> _openCommands = new()
		{
			InputLibrary.menuConfirm,
			InputLibrary.enter,
			InputLibrary.enter2
		};

		protected ModPopupInput(OptionsSelectorElement element, IModMenu menu)
			: base(element, menu)
		{
			SelectorElement = element;
			SetupCommands();
		}

		private void SetupCommands()
		{
			var listenerObject = new GameObject();
			CommandListener = listenerObject.AddComponent<ModCommandListener>();
			_openCommands.ForEach(CommandListener.AddToListener);
			CommandListener.OnNewlyPressed += OnOpenCommand;
		}

		protected void Subscribe(IModButtonBase button) => button.OnClick += Open;

		protected virtual void OnOpenCommand(IInputCommands command)
		{
			if (IsSelected && _openCommands.Contains(command))
			{
				command.ConsumeInput();
				Open();
			}
		}

		protected virtual void Open() => 
			EventSystem.current.SetSelectedGameObject(SelectorElement.gameObject);
	}
}
