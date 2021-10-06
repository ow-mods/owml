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

		public void AddTab(IModTabMenu tabMenu)
		{
			_tabMenus.Add(tabMenu);
			var tabs = _tabMenus.Select(x => x.TabButton).ToArray();
			Menu.SetValue("_menuTabs", tabs);
			AddSelectablePair(tabMenu);
			var parent = tabs[0].transform.parent;
			tabMenu.TabButton.transform.parent = parent;
			UpdateTabNavigation();
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
			for (var i = 0; i < _tabMenus.Count; i++)
			{
				var leftIndex = (i - 1 + _tabMenus.Count) % _tabMenus.Count;
				var rightIndex = (i + 1) % _tabMenus.Count;
				_tabMenus[i].TabButton.GetComponent<Button>().navigation = new Navigation
				{
					selectOnLeft = _tabMenus[leftIndex].TabButton.GetComponent<Button>(),
					selectOnRight = _tabMenus[rightIndex].TabButton.GetComponent<Button>(),
					mode = Navigation.Mode.Explicit
				};
			}
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
