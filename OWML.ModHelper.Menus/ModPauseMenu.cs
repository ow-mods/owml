using OWML.Common;
using OWML.ModHelper.Events;
using UnityEngine;

namespace OWML.ModHelper.Menus
{
    public class ModPauseMenu : ModPopupMenu
    {
        private readonly IModLogger _logger;
        private readonly IModConsole _console;

        public ModPauseMenu(IModLogger logger, IModConsole console, IModEvents events) : base(logger, console)
        {
            _logger = logger;
            _console = console;
            events.Subscribe<PauseMenuManager>(Common.Events.AfterStart);
            events.OnEvent += OnEvent;
        }

        private void OnEvent(MonoBehaviour behaviour, Common.Events ev)
        {
            if (behaviour.GetType() == typeof(PauseMenuManager) && ev == Common.Events.AfterStart)
            {
                _console.WriteLine("Pause menu started");
                var menu = behaviour.GetValue<Menu>("_pauseMenu");
                Initialize(menu);
            }
        }

    }
}
