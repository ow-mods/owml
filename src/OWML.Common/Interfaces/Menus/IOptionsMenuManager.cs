using UnityEngine;

namespace OWML.Common
{
	public interface IOptionsMenuManager
	{
		/// <summary>
		/// Creates a tab with a standard layout - for example, the Audio tab.
		/// </summary>
		/// <param name="name">The name of the tab.</param>
		public (Menu menu, TabButton button) CreateStandardTab(string name);

		/// <summary>
		/// Creates a tab that has sub tabs - for example, the Input tab.
		/// </summary>
		/// <param name="name">The name of the tab.</param>
		public (TabbedSubMenu menu, TabButton button) CreateTabWithSubTabs(string name);

		/// <summary>
		/// Adds a sub-tab to a given TabbedSubMenu.
		/// </summary>
		/// <param name="menu">The menu to add the sub-tab to.</param>
		/// <param name="name">The name of the sub-tab.</param>
		/// <returns>The sub-tab.</returns>
		public (Menu subTabMenu, TabButton subTabButton) AddSubTab(TabbedSubMenu menu, string name);

		/// <summary>
		/// Opens the options menu to the given tab.
		/// </summary>
		/// <param name="tab">Which tab to open.</param>
		public void OpenOptionsAtTab(TabButton button);

		/// <summary>
		/// Adds a checkbox (boolean) input to a given menu.
		/// </summary>
		/// <param name="menu">The menu to add the input to.</param>
		/// <param name="label">The name of the control.</param>
		/// <param name="tooltip">The description of the control.</param>
		/// <param name="initialValue">The value the input should be set to upon creation.</param>
		/// <returns></returns>
		public IOWMLToggleElement AddCheckboxInput(Menu menu, string label, string tooltip, bool initialValue);

		/// <summary>
		/// Adds a toggle (boolean) input to a given menu. Different from a checkbox in that the user is given two named choices, instead of just an input.
		/// </summary>
		/// <param name="menu">The menu to add the input to.</param>
		/// <param name="label">The name of the input.</param>
		/// <param name="leftButtonString">The name of the first (<see langword="false"/>) choice.</param>
		/// <param name="rightButtonString">The name of the second (<see langword="true"/>) choice.</param>
		/// <param name="tooltip">The description of the input.</param>
		/// <param name="initialValue">The value the input should be set to upon creation.</param>
		/// <returns></returns>
		public IOWMLTwoButtonToggleElement AddToggleInput(Menu menu, string label, string leftButtonString, string rightButtonString, string tooltip, bool initialValue);

		/// <summary>
		/// Adds an input to select between multiple choices.
		/// </summary>
		/// <param name="menu">The menu to add the input to.</param>
		/// <param name="label">The name of the input.</param>
		/// <param name="options">The list of options to choose from.</param>
		/// <param name="tooltip">The description of the input.</param>
		/// <param name="loopsAround">Whether or not the selection will roll back to the first choice upon going past the last choice.</param>
		/// <param name="initialValue">The index of <param name="options"> that the input should be set to upon creation.</param>
		/// <returns></returns>
		public IOWMLOptionsSelectorElement AddSelectorInput(Menu menu, string label, string[] options, string tooltip, bool loopsAround, int initialValue);

		/// <summary>
		/// Adds an sliding input.
		/// </summary>
		/// <param name="menu">The menu to add the input to.</param>
		/// <param name="label">The name of the input.</param>
		/// <param name="lower">The lower value of the slider.</param>
		/// <param name="upper">The upper value of the slider.</param>
		/// <param name="tooltip">The description of the input.</param>
		/// <param name="initialValue">The starting value of the sider.</param>
		/// <returns></returns>
		public IOWMLSliderElement AddSliderInput(Menu menu, string label, float lower, float upper, string tooltip, float initialValue);

		/// <summary>
		/// Adds a visual seperator.
		/// </summary>
		/// <param name="menu">The menu to add to.</param>
		/// <param name="dots">Whether the seperator should have a line of dots or not.</param>
		/// <returns></returns>
		public GameObject AddSeparator(Menu menu, bool dots);

		/// <summary>
		/// Creates a button input to a menu.
		/// </summary>
		/// <param name="menu">The menu to add the input to.</param>
		/// <param name="buttonLabel">The label of the button.</param>
		/// <param name="tooltip">The description of the input.</param>
		/// <param name="side">Where to place the button in the menu.</param>
		/// <returns></returns>
		public SubmitAction CreateButton(Menu menu, string buttonLabel, string tooltip, MenuSide side);

		public SubmitAction CreateButtonWithLabel(Menu menu, string label, string buttonLabel, string tooltip);

		/// <summary>
		/// Removes a tab from a menu. This removes both the menu, and the tab for the menu.
		/// Does not work for sub-tabs.
		/// </summary>
		/// <param name="tab">The tab to remove.</param>
		public void RemoveTab(Menu tab);

		/// <summary>
		/// Create a visual label in a menu.
		/// </summary>
		/// <param name="menu">The menu to add the label to.</param>
		/// <param name="label">The text of the label.</param>
		public void CreateLabel(Menu menu, string label);
	}
}
