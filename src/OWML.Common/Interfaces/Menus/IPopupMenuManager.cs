using OWML.Common.Interfaces.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWML.Common
{
	public interface IPopupMenuManager
	{
		public void RegisterStartupPopup(string message);

		public PopupMenu CreateTwoChoicePopup(string message, string confirmText, string cancelText);

		public PopupMenu CreateInfoPopup(string message, string continueButtonText);

		public IOWMLThreeChoicePopupMenu CreateThreeChoicePopup(string message, string confirm1Text, string confirm2Text, string cancelText);

		public IOWMLPopupInputMenu CreateInputFieldPopup(string message, string placeholderMessage, string confirmText, string cancelText);
	}
}
