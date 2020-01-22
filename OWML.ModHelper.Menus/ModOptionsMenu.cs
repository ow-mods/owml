using System.Collections.Generic;
using System.Linq;
using OWML.Common;
using OWML.Common.Menus;
using OWML.ModHelper.Events;

namespace OWML.ModHelper.Menus
{
    public class ModOptionsMenu : ModPopupMenu, IModTabbedMenu
    {
        public IModTabMenu GameplayTab { get; private set; }
        public IModTabMenu AudioTab { get; private set; }
        public IModTabMenu InputTab { get; private set; }
        public IModTabMenu GraphicsTab { get; private set; }

        private readonly IModLogger _logger;
        private readonly IModConsole _console;

        public new TabbedMenu Menu { get; private set; }

        private List<IModTabMenu> _tabMenus;

        public ModOptionsMenu(IModLogger logger, IModConsole console) : base(logger, console)
        {
            _logger = logger;
            _console = console;
        }

        public void Initialize(TabbedMenu menu)
        {
            base.Initialize(menu);
            Menu = menu;

            var tabButtons = Menu.GetValue<TabButton[]>("_menuTabs");
            _tabMenus = new List<IModTabMenu>();
            foreach (var tabButton in tabButtons)
            {
                var tabMenu = new ModTabMenu(_logger, _console, this);
                tabMenu.Initialize(tabButton);
                _tabMenus.Add(tabMenu);
            }

            GameplayTab = _tabMenus.Single(x => x.TabButton.name == "Button-GamePlay");
            AudioTab = _tabMenus.Single(x => x.TabButton.name == "Button-Audio");
            InputTab = _tabMenus.Single(x => x.TabButton.name == "Button-Input");
            GraphicsTab = _tabMenus.Single(x => x.TabButton.name == "Button-Graphics");

            InvokeOnInit();
        }

        public void AddTab(IModTabMenu tabMenu)
        {
            _tabMenus.Add(tabMenu);
            var tabs = _tabMenus.Select(x => x.TabButton).ToArray();
            Menu.SetValue("_menuTabs", tabs);
            var parent = tabs[0].transform.parent;
            tabMenu.TabButton.transform.parent = parent;
        }

        public new IModTabbedMenu Copy()
        {
            return (IModTabbedMenu)base.Copy();
        }

    }
}
