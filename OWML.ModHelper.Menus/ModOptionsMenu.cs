using System.Linq;
using OWML.Common;
using OWML.Common.Menus;
using OWML.ModHelper.Events;

namespace OWML.ModHelper.Menus
{
    public class ModOptionsMenu : ModPopupMenu, IModTabbedMenu
    {
        public IModTabMenu GameplayTab { get; }
        public IModTabMenu AudioTab { get; }
        public IModTabMenu InputTab { get; }
        public IModTabMenu GraphicsTab { get; }

        private readonly IModLogger _logger;
        private readonly IModConsole _console;

        public new TabbedMenu Menu { get; private set; }

        private TabButton[] _tabButtons;

        public ModOptionsMenu(IModLogger logger, IModConsole console) : base(logger, console)
        {
            _logger = logger;
            _console = console;
            GameplayTab = new ModTabMenu(logger, console, this);
            AudioTab = new ModTabMenu(logger, console, this);
            InputTab = new ModTabMenu(logger, console, this);
            GraphicsTab = new ModTabMenu(logger, console, this);
        }

        public void Initialize(TabbedMenu menu)
        {
            base.Initialize(menu);

            Menu = menu;
            _console.WriteLine("init of options menu");

            _tabButtons = menu.GetValue<TabButton[]>("_menuTabs");

            GameplayTab.Initialize(GetTabButton("Button-GamePlay"));
            AudioTab.Initialize(GetTabButton("Button-Audio"));
            InputTab.Initialize(GetTabButton("Button-Input"));
            GraphicsTab.Initialize(GetTabButton("Button-Graphics"));

            OnInit?.Invoke();
            
            _console.WriteLine("options: inited");
        }

        public TabButton GetTabButton(string name)
        {
            var tabButton = _tabButtons.FirstOrDefault(x => x.name == name);
            if (tabButton == null)
            {
                _console.WriteLine("Error: could not find tab for " + name);
                return null;
            }
            return tabButton;
        }

        public new IModTabbedMenu Copy()
        {
            return (IModTabbedMenu)base.Copy();
        }

    }
}
