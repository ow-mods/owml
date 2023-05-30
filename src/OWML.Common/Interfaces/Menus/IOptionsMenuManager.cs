using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

		public IOWMLToggleElement AddCheckboxInput(Menu menu, string label, string tooltip, bool initialValue);

		public IOWMLTwoButtonToggleElement AddToggleInput(Menu menu, string label, string leftButtonString, string rightButtonString, string tooltip, bool initialValue);

		public IOWMLOptionsSelectorElement AddSelectorInput(Menu menu, string label, string[] options, string tooltip, bool loopsAround, int initialValue);

		public GameObject AddSeparator(Menu menu, bool dots);

		public SubmitAction CreateButton(Menu menu, string label, string tooltip, MenuSide side);

		public void RemoveTab(Menu tab);
	}
}
