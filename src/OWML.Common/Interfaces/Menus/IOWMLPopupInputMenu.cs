using UnityEngine.UI;

namespace OWML.Common.Interfaces.Menus
{
	public interface IOWMLPopupInputMenu
	{
		public delegate bool InputPopupValidateCharEvent(string input, int charIndex, char addedChar);

		public event InputPopupValidateCharEvent OnInputPopupValidateChar;

		public void EnableMenu(bool value);

		public string GetInputText();

		public InputField GetInputField();

		public void SetInputFieldPlaceholderText(string text);

		public event PopupMenu.PopupConfirmEvent OnPopupConfirm;
		public event Menu.ActivateMenuEvent OnActivateMenu;
		public event Menu.DeactivateMenuEvent OnDeactivateMenu;
	}
}
