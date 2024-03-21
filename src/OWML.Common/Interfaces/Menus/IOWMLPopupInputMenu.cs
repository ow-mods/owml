using UnityEngine.UI;

namespace OWML.Common.Interfaces.Menus
{
	public interface IOWMLPopupInputMenu
	{
		public event PopupInputMenu.InputPopupValidateCharEvent OnInputPopupValidateChar;

		public void EnableMenu(bool value);

		public string GetInputText();

		public InputField GetInputField();

		public void SetInputFieldPlaceholderText(string text);

		public event PopupMenu.PopupConfirmEvent OnPopupConfirm;
	}
}
