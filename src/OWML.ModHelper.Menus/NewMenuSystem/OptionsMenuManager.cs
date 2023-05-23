using OWML.Common;
using OWML.ModHelper.Menus.NewMenuSystem.Interfaces;
using OWML.Utils;
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
		private readonly IModConsole _console;

		public OptionsMenuManager(IModConsole console)
		{
			_console = console;
		}

		public void CreateStandardTab(string name)
		{

		}

		public TabbedSubMenu CreateTabWithSubTabs(string name)
		{
			_console.WriteLine($"CreateTabWithSubTabs");
			var existingTabbedSubMenu = Resources.FindObjectsOfTypeAll<TabbedSubMenu>().Single(x => x.name == "GameplayMenu").gameObject;

			var newSubMenu = Object.Instantiate(existingTabbedSubMenu);
			newSubMenu.transform.parent = existingTabbedSubMenu.transform.parent;
			newSubMenu.transform.localScale = Vector3.one;
			var rectTransform = newSubMenu.GetComponent<RectTransform>();
			rectTransform.anchorMin = existingTabbedSubMenu.GetComponent<RectTransform>().anchorMin;
			rectTransform.anchorMax = existingTabbedSubMenu.GetComponent<RectTransform>().anchorMax;
			rectTransform.anchoredPosition3D = existingTabbedSubMenu.GetComponent<RectTransform>().anchoredPosition3D;
			rectTransform.offsetMin = existingTabbedSubMenu.GetComponent<RectTransform>().offsetMin;
			rectTransform.offsetMax = existingTabbedSubMenu.GetComponent<RectTransform>().offsetMax;
			rectTransform.sizeDelta = existingTabbedSubMenu.GetComponent<RectTransform>().sizeDelta;

			var tabbedSubMenu = newSubMenu.GetComponent<TabbedSubMenu>();

			var tabButton = CreateTabButton(name, tabbedSubMenu);

			var optionsMenu = GameObject.Find("TitleMenu").transform.Find("OptionsCanvas").Find("OptionsMenu-Panel").GetComponent<TabbedMenu>();
			optionsMenu._subMenus = optionsMenu._subMenus.Add(tabbedSubMenu);
			optionsMenu._menuTabs = optionsMenu._menuTabs.Add(tabButton);
			optionsMenu._tabSelectablePairs = optionsMenu._tabSelectablePairs.Add(
				new TabbedMenu.TabSelectablePair()
				{
					tabButton = tabButton,
					selectable = null
				});

			return tabbedSubMenu;
		}

		private TabButton CreateTabButton(string name, TabbedMenu menu)
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
			tabButton._tabbedMenu = menu ?? throw new System.Exception("Menu cannot be null.");

			return tabButton;
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
