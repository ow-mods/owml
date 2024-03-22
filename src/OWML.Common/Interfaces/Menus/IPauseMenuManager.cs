using System;
using UnityEngine;

namespace OWML.Common
{
	public interface IPauseMenuManager
	{
		/// <summary>
		/// Called when the pause menu is opened.
		/// </summary>
		event Action PauseMenuOpened;

		/// <summary>
		/// Called when the pause menu is closed.
		/// </summary>
		event Action PauseMenuClosed;

		/// <summary>
		/// Makes another list of buttons that can be seen in the pause menu.
		/// </summary>
		/// <param name="title">The title that appears at the top.</param>
		public Menu MakePauseListMenu(string title);

		/// <summary>
		/// Makes a button on a pause menu.
		/// </summary>
		/// <param name="name">The text of the button.</param>
		/// <param name="index">The position index of the button.</param>
		/// <param name="fromTop">Whether the position index should be from the top or from the bottom.</param>
		/// <param name="customMenu">If given, this is the custom menu this button will appear on. If not given, the button will appear on the default pause menu.</param>
		public SubmitAction MakeSimpleButton(string name, int index, bool fromTop, Menu customMenu = null);

		/// <summary>
		/// Makes a button on a pause menu that opens a different menu.
		/// </summary>
		/// <param name="name">The text of the button.</param>
		/// <param name="menuToOpen">The menu to be opened when this button is pressed.</param>
		/// <param name="index">The position index of the button.</param>
		/// <param name="fromTop">Whether the position index should be from the top or from the bottom.</param>
		/// <param name="customMenu">If given, this is the custom menu this button will appear on. If not given, the button will appear on the default pause menu.</param>
		public SubmitAction MakeMenuOpenButton(string name, Menu menuToOpen, int index, bool fromTop, Menu customMenu = null);

		/// <summary>
		/// Set the text of a given button.
		/// </summary>
		/// <param name="button">The button to set the text of.</param>
		/// <param name="text">The text to set the button to.</param>
		public void SetButtonText(SubmitAction button, string text);

		/// <summary>
		/// Sets the position of a given button.
		/// </summary>
		/// <param name="button">The button to set the position of.</param>
		/// <param name="index">The new position index of the button.</param>
		/// <param name="fromTop">Whether the position index should be from the top or from the bottom.</param>
		public void SetButtonIndex(SubmitAction button, int index, bool fromTop);
	}
}
