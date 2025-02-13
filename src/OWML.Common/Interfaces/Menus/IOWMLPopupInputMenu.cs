using System;
using UnityEngine.UI;

namespace OWML.Common.Interfaces.Menus
{
	public interface IOWMLPopupInputMenu
	{
		[Obsolete("Use OnValidateChar instead.")]
		public event PopupInputMenu.InputPopupValidateCharEvent OnInputPopupValidateChar;

		public delegate bool InputPopupValidateCharEvent(string input, int charIndex, char addedChar);
		public event InputPopupValidateCharEvent OnValidateChar;

		public void EnableMenu(bool value);

		public string GetInputText();

		public InputField GetInputField();

		public void SetInputFieldPlaceholderText(string text);

		public void SetText(string message, string placeholderMessage, string confirmText, string cancelText);

		public event PopupMenu.PopupConfirmEvent OnPopupConfirm;
		public event Menu.ActivateMenuEvent OnActivateMenu;
		public event Menu.DeactivateMenuEvent OnDeactivateMenu;
	}
}
