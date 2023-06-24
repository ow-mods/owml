using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWML.Common.Interfaces.Menus
{
	public interface IOWMLPopupInputMenu
	{
		public void EnableMenu(bool value);

		public string GetInputText();

		public event PopupMenu.PopupConfirmEvent OnPopupConfirm;
	}
}
