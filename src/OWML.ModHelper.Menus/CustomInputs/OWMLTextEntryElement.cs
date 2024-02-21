using System.Linq;
using OWML.Common;
using OWML.Common.Interfaces.Menus;

namespace OWML.ModHelper.Menus.CustomInputs
{
	internal class OWMLTextEntryElement : OWMLMenuValueOption, IOWMLTextEntryElement
	{
		public event TextEntryConfirmEvent OnConfirmEntry;

		public bool IsNumeric;

		private IOWMLPopupInputMenu _popup;

		internal void RegisterPopup(IOWMLPopupInputMenu popup)
		{
			_popup = popup;
			_popup.OnPopupConfirm += () => OnConfirmEntry();
		}

		public string GetInputText()
		{
			return _popup.GetInputText();
		}

		public void SetCurrentValue(string text)
		{
			gameObject.GetComponentsInChildren<MenuOption>().First(x => x != this)._label.text = text;
			_popup.GetInputField().text = text;
			OnConfirmEntry();
		}
	}
}
