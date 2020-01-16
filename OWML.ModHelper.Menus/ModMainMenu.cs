using System.Linq;
using OWML.Common;
using OWML.Common.Menus;
using OWML.ModHelper.Events;
using UnityEngine;

namespace OWML.ModHelper.Menus
{
    public class ModMainMenu : ModMenu, IModMainMenu
    {
        public IModTabbedMenu OptionsMenu { get; private set; }

        public IModButton ResumeExpeditionButton { get; private set; }
        public IModButton NewExpeditionButton { get; private set; }
        public IModButton OptionsButton { get; private set; }
        public IModButton ViewCreditsButton { get; private set; }
        public IModButton SwitchProfileButton { get; private set; }
        public IModButton QuitButton { get; private set; }

        private TitleAnimationController _anim;

        private readonly IModLogger _logger;
        private readonly IModConsole _console;

        public ModMainMenu(IModLogger logger, IModConsole console) : base(logger, console)
        {
            _logger = logger;
            _console = console;
            OptionsMenu = new ModOptionsMenu(logger, console);
        }

        public void Initialize(TitleScreenManager titleScreenManager)
        {
            _anim = titleScreenManager.GetComponent<TitleAnimationController>();
            var menu = titleScreenManager.GetValue<Menu>("_mainMenu");
            Initialize(menu);
            ResumeExpeditionButton = GetButton("Button-ResumeGame");
            NewExpeditionButton = GetButton("Button-NewGame");
            OptionsButton = GetButton("Button-Options");
            ViewCreditsButton = GetButton("Button-Credits");
            SwitchProfileButton = GetButton("Button-Profile");
            QuitButton = GetButton("Button-Exit");
            var tabbedMenu = titleScreenManager.GetValue<TabbedMenu>("_optionsMenu");
            OptionsMenu.Initialize(tabbedMenu);
        }

        public override void AddButton(IModButton button, int index)
        {
            base.AddButton(button, index);
            var fadeControllers = Buttons.OrderBy(x => x.Index).Select(x => new CanvasGroupFadeController
            {
                group = x.Button.GetComponent<CanvasGroup>()
            });
            _anim.SetValue("_buttonFadeControllers", fadeControllers.ToArray());
        }

    }
}
