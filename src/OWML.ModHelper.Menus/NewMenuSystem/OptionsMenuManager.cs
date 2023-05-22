using OWML.ModHelper.Menus.NewMenuSystem.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus.NewMenuSystem
{
	internal class OptionsMenuManager : IOptionsMenuManager
	{
		public void CreateStandardTab(string name)
		{

		}

		public void CreateTabWithSubTabs(string name)
		{
			var existingTabbedSubMenu = Resources.FindObjectsOfTypeAll<TabbedSubMenu>().Single(x => x.name == "GameplayMenu").gameObject;


			CreateTabButton(name, null);
		}

		private void CreateTabButton(string name, TabbedMenu menu)
		{
			var existingButton = Resources.FindObjectsOfTypeAll<TabButton>().Single(x => x.name == "Button-Graphics");

			var newButton = Object.Instantiate(existingButton);
			newButton.transform.parent = existingButton.transform.parent;
			newButton.transform.localScale = Vector3.one;
			newButton.transform.SetSiblingIndex(newButton.transform.parent.childCount - 2);
			newButton.name = $"Button-{name}";

			RecalculateNavigation(newButton.transform.parent.GetComponentsInChildren<Button>(true).ToList());

			var text = newButton.GetComponentInChildren<Text>();
			Object.Destroy(text.GetComponent<LocalizedText>());
			text.text = name;

			var tabButton = newButton.GetComponent<TabButton>();
			tabButton._tabbedMenu = menu;
		}

		private void RecalculateNavigation(List<Button> tabButtons)
		{
			// deal with edge rollover
			SetSelectOnLeft(tabButtons[0], tabButtons.Last());
			SetSelectOnRight(tabButtons.Last(), tabButtons[0]);

			for (var i = 0; i < tabButtons.Count; i++)
			{
				if (i == 0)
				{
					SetSelectOnRight(tabButtons[i], tabButtons[i + 1]);
				}
				else if (i == tabButtons.Count - 1)
				{
					SetSelectOnLeft(tabButtons[i], tabButtons[i - 1]);
				}
				else
				{
					SetSelectOnLeft(tabButtons[i], tabButtons[i - 1]);
					SetSelectOnRight(tabButtons[i], tabButtons[i + 1]);
				}
			}
		}

		private void SetSelectOnLeft(Button button, Selectable selectable)
		{
			var navigation = button.navigation;
			navigation.selectOnLeft = selectable;
			button.navigation = navigation;
		}

		private void SetSelectOnRight(Button button, Selectable selectable)
		{
			var navigation = button.navigation;
			navigation.selectOnRight = selectable;
			button.navigation = navigation;
		}
	}
}
