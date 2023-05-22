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
		public void CreateTabWithSubTabs(string name);
	}
}
