using System.Linq;
using OWML.Common;
using OWML.ModHelper.Events;

namespace OWML.ModHelper.Menus
{
    public class ModOptionsMenu : ModPopupMenu, IModTabbedMenu
    {
        public IModPopupMenu GamePlay { get; }
        public IModPopupMenu Audio { get; }
        public IModPopupMenu Input { get; }
        public IModPopupMenu Graphics { get; }

        private readonly IModLogger _logger;
        private readonly IModConsole _console;

        public ModOptionsMenu(IModLogger logger, IModConsole console) : base(logger, console)
        {
            _logger = logger;
            _console = console;
            GamePlay = new ModPopupMenu(logger, console);
            Audio = new ModPopupMenu(logger, console);
            Input = new ModPopupMenu(logger, console);
            Graphics = new ModPopupMenu(logger, console);
        }

        public override void Initialize(Menu menu)
        {
            _console.WriteLine("init of options");
            base.Initialize(menu);
            _console.WriteLine("after base init of options");
            var tabs = menu.GetValue<TabButton[]>("_menuTabs");
            if (tabs == null || tabs.Length == 0)
            {
                _console.WriteLine("Error: _menuTabs is null or empty");
                return;
            }
            GamePlay.Initialize(GetTab(tabs, "GamePlay"));
            Audio.Initialize(GetTab(tabs, "Audio"));
            Input.Initialize(GetTab(tabs, "Input"));
            Graphics.Initialize(GetTab(tabs, "Graphics"));
        }

        private Menu GetTab(TabButton[] tabs, string name)
        {
            _console.WriteLine("get tab of " + name);
            var tab = tabs.FirstOrDefault(x => x.name == $"Button-{name}");
            if (tab == null)
            {
                _console.WriteLine("Error: could not find tab for " + name);
                return null;
            }
            var menu = tab.GetValue<Menu>("_tabbedMenu");
            if (menu == null)
            {
                _console.WriteLine("Error: menu is null for tab " + name);
                return null;
            }
            return menu;
        }

        public new IModTabbedMenu Copy()
        {
            return (IModTabbedMenu)base.Copy();
        }

    }
}
