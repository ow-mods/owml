using System.Collections.Generic;
using System.Linq;
using OWML.Common;
using OWML.Common.Menus;
using OWML.Utils;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
	public class ModOptionsMenu : ModPopupMenu, IModTabbedMenu
	{
		public IModTabMenu GameplayTab { get; private set; }

		public IModTabMenu AudioTab { get; private set; }

		public IModTabMenu InputTab { get; private set; }

		public IModTabMenu GraphicsTab { get; private set; }

		public new TabbedMenu Menu { get; private set; }

		private List<IModTabMenu> _tabMenus;

		private GraphicRaycaster _raycaster;

		public ModOptionsMenu(IModConsole console)
			: base(console)
		{
		}

		public void Initialize(TabbedMenu menu)
		{
			base.Initialize(menu);
			Menu = menu;

			_raycaster = Menu.transform.parent.GetComponent<GraphicRaycaster>();
			var tabButtons = Menu.GetValue<TabButton[]>("_menuTabs");
			_tabMenus = new List<IModTabMenu>();
			foreach (var tabButton in tabButtons)
			{
				var tabMenu = new ModTabMenu(this, Console);
				tabMenu.Initialize(tabButton);
				_tabMenus.Add(tabMenu);
			}

			GameplayTab = GetTab("Button-GamePlayOG");
			AudioTab = GetTab("Button-Audio");
			InputTab = GetTab("Button-Input");
			GraphicsTab = GetTab("Button-Graphics");

			InvokeOnInit();
		}

		public void AddTab(IModTabMenu tabMenu, bool enable = true)
		{
			_tabMenus.Add(tabMenu);
			var tabs = _tabMenus.Select(x => x.TabButton).ToArray();
			Menu.SetValue("_menuTabs", tabs);
			AddSelectablePair(tabMenu);
			var parent = tabs[0].transform.parent;
			tabMenu.TabButton.transform.parent = parent;
			if (enable)
			{
				UpdateTabNavigation();
				return;
			}
			var navigation = tabMenu.TabButton.GetSelectable().navigation;
			navigation.selectOnLeft = null;
			navigation.selectOnRight = null;
			tabMenu.TabButton.GetSelectable().navigation = navigation;
			tabMenu.HideButton();
		}

		public void SetIsBlocking(bool isBlocking) =>
			_raycaster.gameObject.SetActive(isBlocking);

		private void AddSelectablePair(IModTabMenu tabMenu)
		{
			var selectablePairs = Menu.GetValue<TabbedMenu.TabSelectablePair[]>("_tabSelectablePairs").ToList();
			selectablePairs.Add(new TabbedMenu.TabSelectablePair
			{
				tabButton = tabMenu.TabButton,
				selectable = tabMenu.TabButton.GetComponent<Selectable>()
			});
			Menu.SetValue("_tabSelectablePairs", selectablePairs.ToArray());
		}

		private void UpdateTabNavigation()
		{
			Selectable previous = null, first = null;
			for (var i = 0; i < _tabMenus.Count; i++)
			{
				var current = _tabMenus[i].TabButton.GetSelectable();
				if (!(current?.gameObject.activeSelf ?? false))
				{
					continue;
				}
				if (first == null)
				{
					first = current;
				}
				if (previous != null)
				{
					LinkNavigation(previous, current);
				}
				previous = current;
			}
			if (first != null && previous != null)
			{
				LinkNavigation(previous, first);
			}
		}

		private void LinkNavigation(Selectable first, Selectable second)
		{
			var prevNav = first.navigation;
			var curNav = second.navigation;
			prevNav.selectOnRight = second;
			curNav.selectOnLeft = first;
			second.navigation = curNav;
			first.navigation = prevNav;
		}

		public new IModTabbedMenu Copy()
		{
			return (IModTabbedMenu)base.Copy();
		}

		private IModTabMenu GetTab(string tabName)
		{
			return _tabMenus.Single(x => x.TabButton.name == tabName);
		}
	}
}
