using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWML.ModHelper.Menus.NewMenuSystem.Interfaces
{
	public interface ITitleMenuManager
	{
		/// <summary>
		/// Creates a button on the main title screen.
		/// </summary>
		/// <param name="name">The string to display on the button.</param>
		/// <param name="index">The index of the button. 0 means it should be the first button, 1 means the second etc.</param>
		/// <param name="fromTop">Whethether the index should be top-down or bottom-up.</param>
		/// <returns>The SubmitAction of the button.</returns>
		SubmitAction CreateTitleButton(string name, int index = 0, bool fromTop = false);

		/// <summary>
		/// Set the string to display on the button.
		/// </summary>
		/// <param name="button">The button to change the text of.</param>
		/// <param name="text">The text to change it to.</param>
		void SetButtonText(SubmitAction button, string text);

		/// <summary>
		/// Change the position of a button in the main menu list.
		/// </summary>
		/// <param name="buttonAction">The button to move.</param>
		/// <param name="index">The index to change it to.</param>
		/// <param name="fromTop">Whether to base the index from the bottom or top. If true, then an index of 0 will mean the top.</param>
		void SetButtonIndex(SubmitAction buttonAction, int index, bool fromTop);
	}
}
