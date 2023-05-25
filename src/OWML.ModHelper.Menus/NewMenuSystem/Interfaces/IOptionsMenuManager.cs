using OWML.ModHelper.Menus.CustomInputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWML.ModHelper.Menus.NewMenuSystem.Interfaces
{
	public interface IOptionsMenuManager
	{
		/// <summary>
		/// Creates a tab with a standard layout - for example, the Audio tab.
		/// </summary>
		/// <param name="name">The name of the tab.</param>
		public void CreateStandardTab(string name);

		/// <summary>
		/// Creates a tab that has sub tabs - for example, the Input tab.
		/// </summary>
		/// <param name="name">The name of the tab.</param>
		public TabbedSubMenu CreateTabWithSubTabs(string name);

		/// <summary>
		/// Adds a sub-tab to a given TabbedSubMenu.
		/// </summary>
		/// <param name="menu">The menu to add the sub-tab to.</param>
		/// <param name="name">The name of the sub-tab.</param>
		/// <returns>The sub-tab.</returns>
		public Menu AddSubTab(TabbedSubMenu menu, string name);

		/// <summary>
		/// Opens the options menu to the given tab.
		/// </summary>
		/// <param name="tab">Which tab to open.</param>
		public void OpenOptionsAtTab(Menu tab);

		public OWMLToggleElement AddCheckboxInput(Menu menu, string label, string tooltip, bool initialValue);

		public OWMLTwoButtonToggleElement AddToggleInput(Menu menu, string label, string leftButtonString, string rightButtonString, string tooltip, bool initialValue);

		public OWMLOptionsSelectorElement AddSelectorInput(Menu menu, string label, string[] options, string tooltip, bool loopsAround, int initialValue);
	}
}
