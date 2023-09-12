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
		/// <summary>
		/// Creates a popup that will appear when the title screen loads. Must be called in Start().
		/// </summary>
		/// <param name="message">The text of the popup.</param>
		public void RegisterStartupPopup(string message);

		/// <summary>
		/// Creates a popup with two buttons to choose from.
		/// </summary>
		/// <param name="message">The message text of the popup.</param>
		/// <param name="confirmText">The text that appears on the left/confirm button.</param>
		/// <param name="cancelText">The text that appears on the right/cancel button.</param>
		/// <returns>The PopupMenu for you to enable/disable, or hook events into.</returns>
		public PopupMenu CreateTwoChoicePopup(string message, string confirmText, string cancelText);

		/// <summary>
		/// Creates a popup with only one button that closes it.
		/// </summary>
		/// <param name="message">The message to display.</param>
		/// <param name="continueButtonText">The text that appears on the singular button.</param>
		/// <returns>The PopupMenu for you to enable/disable, or hook events into.</returns>
		public PopupMenu CreateInfoPopup(string message, string continueButtonText);

		/// <summary>
		/// Creates a popup with three buttons to choose from.
		/// </summary>
		/// <param name="message">The message text of the popup.</param>
		/// <param name="confirm1Text">The text that appears on the left/first confirm button.</param>
		/// <param name="confirm2Text">The text that appears on the center/second confirm button.</param>
		/// <param name="cancelText">The text that appears on the right/cancel button.</param>
		/// <returns>The IOWMLThreeChoicePopupMenu for you to enable/disable, or hook events into.</returns>
		public IOWMLThreeChoicePopupMenu CreateThreeChoicePopup(string message, string confirm1Text, string confirm2Text, string cancelText);

		/// <summary>
		/// Creates a popup with three buttons to choose from.
		/// </summary>
		/// <param name="message">The message text of the popup.</param>
		/// <param name="confirm1Text">The text that appears on the first-from-the-left confirm button.</param>
		/// <param name="confirm2Text">The text that appears on the second-from-the-left confirm button.</param>
		/// <param name="confirm3Text">The text that appears on the third-from-the-left confirm button.</param>
		/// <param name="cancelText">The text that appears on the cancel button.</param>
		/// <returns>The IOWMLFourChoicePopupMenu for you to enable/disable, or hook events into.</returns>
		public IOWMLFourChoicePopupMenu CreateFourChoicePopup(string message, string confirm1Text, string confirm2Text, string confirm3Text, string cancelText);

		public IOWMLPopupInputMenu CreateInputFieldPopup(string message, string placeholderMessage, string confirmText, string cancelText);
	}
}
