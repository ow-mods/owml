using OWML.Common;
using OWML.ModHelper.Menus.CustomInputs;
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

			Object.Destroy(tabbedSubMenu._subMenus[0].gameObject);
			Object.Destroy(tabbedSubMenu._subMenus[1].gameObject);
			Object.Destroy(tabbedSubMenu._subMenus[2].gameObject);
			tabbedSubMenu._subMenus = null;
			Object.Destroy(tabbedSubMenu._tabSelectablePairs[0].tabButton.gameObject);
			Object.Destroy(tabbedSubMenu._tabSelectablePairs[1].tabButton.gameObject);
			Object.Destroy(tabbedSubMenu._tabSelectablePairs[2].tabButton.gameObject);
			tabbedSubMenu._tabSelectablePairs = null;
			tabbedSubMenu._selectOnActivate = null;

			return tabbedSubMenu;
		}

		public Menu AddSubTab(TabbedSubMenu menu, string name)
		{
			var existingTabbedSubMenu = Resources.FindObjectsOfTypeAll<TabbedSubMenu>().Single(x => x.name == "GameplayMenu").gameObject;

			var existingSubMenu = existingTabbedSubMenu.GetComponent<TabbedSubMenu>()._subMenus[0].gameObject;
			var existingSubMenuTabButton = existingTabbedSubMenu.GetComponent<TabbedSubMenu>()._tabSelectablePairs[0].tabButton.gameObject;

			var newSubMenuTabButton = Object.Instantiate(existingSubMenuTabButton);
			newSubMenuTabButton.transform.parent = menu.transform.Find("SubMenuTabs");
			newSubMenuTabButton.transform.SetSiblingIndex(newSubMenuTabButton.transform.parent.childCount - 2);
			newSubMenuTabButton.name = $"Button-{name}Tab";
			Object.Destroy(newSubMenuTabButton.GetComponentInChildren<LocalizedText>());
			newSubMenuTabButton.GetComponentInChildren<Text>().text = name;

			var newSubMenu = Object.Instantiate(existingSubMenu);
			newSubMenu.transform.parent = menu.transform;
			newSubMenu.name = $"Menu{name}";

			var rt = newSubMenu.GetComponent<RectTransform>();
			var ert = existingSubMenu.GetComponent<RectTransform>();
			rt.anchorMin = ert.anchorMin;
			rt.anchorMax = ert.anchorMax;
			rt.anchoredPosition3D = ert.anchoredPosition3D;
			rt.offsetMin = ert.offsetMin;
			rt.offsetMax = ert.offsetMax;
			rt.sizeDelta = ert.sizeDelta;

			if (menu._selectOnActivate == null)
			{
				menu._selectOnActivate = newSubMenuTabButton.GetComponent<Button>();
			}

			menu._subMenus = menu._subMenus == null
				? (new[] { newSubMenu.GetComponent<Menu>() })
				: menu._subMenus.Add(newSubMenu.GetComponent<Menu>());

			menu._tabSelectablePairs = menu._tabSelectablePairs == null
				? (new TabbedMenu.TabSelectablePair[] { new TabbedMenu.TabSelectablePair() { tabButton = newSubMenuTabButton.GetComponent<TabButton>() } })
				: menu._tabSelectablePairs.Add(new TabbedMenu.TabSelectablePair() { tabButton = newSubMenuTabButton.GetComponent<TabButton>() });

			newSubMenuTabButton.GetComponent<TabButton>()._tabbedMenu = newSubMenu.GetComponent<Menu>();

			RecalculateNavigation(menu._tabSelectablePairs.Select(x => x.tabButton.GetComponent<Button>()).ToList());

			foreach (var item in newSubMenu.GetComponent<Menu>()._menuOptions)
			{
				Object.Destroy(item.gameObject);
			}

			newSubMenu.GetComponent<Menu>()._menuOptions = new MenuOption[] { };

			return newSubMenu.GetComponent<Menu>();
		}

		public void OpenOptionsAtTab(Menu tab)
		{
			var optionsMenu = GameObject.Find("TitleMenu").transform.Find("OptionsCanvas").Find("OptionsMenu-Panel").GetComponent<TabbedMenu>();
			optionsMenu.EnableMenu(true);

			var tabButton = optionsMenu._menuTabs.Single(x => x._tabbedMenu == tab);
			optionsMenu.SelectTabButton(tabButton);
		}

		public OWMLToggleElement AddCheckboxInput(Menu menu, string label, string tooltip, bool initialValue)
		{
			var existingCheckbox = Resources.FindObjectsOfTypeAll<TabbedSubMenu>()
				.Single(x => x.name == "GameplayMenu").transform
				.Find("MenuGameplayBasic")
				.Find("UIElement-InvertPlayerLook").gameObject;

			var newCheckbox = Object.Instantiate(existingCheckbox);
			newCheckbox.transform.parent = menu.transform;
			newCheckbox.transform.localScale = Vector3.one;
			newCheckbox.transform.name = $"UIElement-{label}";

			var oldCheckbox = newCheckbox.GetComponent<ToggleElement>();

			var customCheckboxScript = newCheckbox.AddComponent<OWMLToggleElement>();
			customCheckboxScript._label = oldCheckbox._label;
			customCheckboxScript._overrideTooltipText = tooltip;
			customCheckboxScript._displayText = oldCheckbox._displayText;
			customCheckboxScript._displayText.text = label;
			customCheckboxScript._toggleGraphic = oldCheckbox._toggleGraphic;
			customCheckboxScript._toggleElementButton = oldCheckbox._toggleElementButton;

			customCheckboxScript.Initialize(initialValue);

			Object.Destroy(oldCheckbox);

			menu._menuOptions = menu._menuOptions.Add(customCheckboxScript);

			return customCheckboxScript;
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
			if (tabButtons.Count == 1)
			{
				SetSelectOnLeft(tabButtons[0], tabButtons[0]);
				SetSelectOnRight(tabButtons[0], tabButtons[0]);
				return;
			}

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
