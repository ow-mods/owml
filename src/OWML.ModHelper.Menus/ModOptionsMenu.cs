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

		private IModTabMenu _defaultTab;

		private int _menuStackCount;

		public ModOptionsMenu(IModConsole console)
			: base(console)
		{
		}

		public void Initialize(TabbedMenu menu, int menuStackCount)
		{
			_menuStackCount = menuStackCount;
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
			_defaultTab = GameplayTab;
			GameplayTab.OnClosed += OnTabClose;
			AudioTab = GetTab("Button-Audio");
			AudioTab.OnClosed += OnTabClose;
			InputTab = GetTab("Button-Input");
			InputTab.OnClosed += OnTabClose;
			GraphicsTab = GetTab("Button-Graphics");
			GraphicsTab.OnClosed += OnTabClose;

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
			tabMenu.OnOpened += () => OnTabOpen(tabMenu);
			tabMenu.OnClosed += OnTabClose;

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

		private void OnTabOpen(IModTabMenu tabMenu)
		{
			Console.WriteLine($"Setting Mod Tab OnActivate {tabMenu.TabButton.gameObject.name}");
			Menu.SetValue("_firstSelectedTabButton", tabMenu.TabButton);
		}

		private void OnTabClose()
		{
			Console.WriteLine($"Checking MenuCount {MenuStackManager.SharedInstance.GetMenuCount()}");
			if (MenuStackManager.SharedInstance.GetMenuCount()<= _menuStackCount)
			{
				Console.WriteLine("Setting default Tab OnActivate");
				Menu.SetValue("_firstSelectedTabButton", _defaultTab.TabButton);
			}
		}

		private void UpdateTabNavigation()
		{
			Selectable previous = null, first = null;
			var tabMenus = _tabMenus
				.Select(tabMenu => tabMenu.TabButton.GetSelectable())
				.Where(current => current?.gameObject.activeSelf ?? false);
			foreach (var current in tabMenus)
			{
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
