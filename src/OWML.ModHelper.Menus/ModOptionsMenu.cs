using System.Collections.Generic;
using System.Linq;
using OWML.Common.Interfaces.Menus;
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

        public IModButton RebindingButton { get; private set; }

        public IModPopupMenu RebindingMenu { get; private set; }

        public new TabbedMenu Menu { get; private set; }

        private List<IModTabMenu> _tabMenus;

        public void Initialize(TabbedMenu menu)
        {
            base.Initialize(menu);
            Menu = menu;

            var tabButtons = Menu.GetValue<TabButton[]>("_menuTabs");
            _tabMenus = new List<IModTabMenu>();
            foreach (var tabButton in tabButtons)
            {
                var tabMenu = new ModTabMenu(this);
                tabMenu.Initialize(tabButton);
                _tabMenus.Add(tabMenu);
            }

            GameplayTab = GetTab("Button-GamePlay");
            AudioTab = GetTab("Button-Audio");
            InputTab = GetTab("Button-Input");
            GraphicsTab = GetTab("Button-Graphics");

            RebindingButton = InputTab.GetTitleButton("UIElement-RemapControls");
            RebindingMenu = GetRebindingMenu();

            InvokeOnInit();
        }

        public void AddTab(IModTabMenu tabMenu)
        {
            _tabMenus.Add(tabMenu);
            var tabs = _tabMenus.Select(x => x.TabButton).ToArray();
            Menu.SetValue("_menuTabs", tabs);
            var parent = tabs[0].transform.parent;
            tabMenu.TabButton.transform.parent = parent;
            UpdateTabNavigation();
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

        private IModPopupMenu GetRebindingMenu()
        {
            var menu = RebindingButton.Button
                .GetComponent<SubmitActionMenu>()
                .GetValue<Menu>("_menuToOpen");
            var rebindingMenu = new ModPopupMenu();
            rebindingMenu.Initialize(menu);
            return rebindingMenu;
        }

    }
}
