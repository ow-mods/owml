using OWML.Common;
using OWML.Common.Menus;
using OWML.ModHelper.Events;

namespace OWML.ModHelper.Menus
{
    public class ModTabMenu : ModPopupMenu, IModTabMenu
    {
        private readonly IModLogger _logger;
        private readonly IModConsole _console;
        private readonly IModTabbedMenu _optionsMenu;

        private TabButton _tabButton;

        public ModTabMenu(IModLogger logger, IModConsole console, IModTabbedMenu optionsMenu) : base(logger, console)
        {
            _logger = logger;
            _console = console;
            _optionsMenu = optionsMenu;
        }

        public void Initialize(TabButton tabButton)
        {
            _tabButton = tabButton;
            var menu = tabButton.GetValue<Menu>("_tabbedMenu");
            Initialize(menu);
            InvokeOnInit();
        }

        public override void Open()
        {
            if (!_optionsMenu.IsOpen)
            {
                _optionsMenu.Open();
            }
            _optionsMenu.Menu.Invoke("SelectTabButton", _tabButton);
        }

    }
}
