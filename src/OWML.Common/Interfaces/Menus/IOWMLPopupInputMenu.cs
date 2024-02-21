using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace OWML.Common.Interfaces.Menus
{
	public interface IOWMLPopupInputMenu
	{
		public event PopupInputMenu.InputPopupValidateCharEvent OnInputPopupValidateChar;

		public void EnableMenu(bool value);

		public string GetInputText();

		public InputField GetInputField();

		public event PopupMenu.PopupConfirmEvent OnPopupConfirm;
	}
}
