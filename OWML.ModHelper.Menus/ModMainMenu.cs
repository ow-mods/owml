using System.Linq;
using OWML.Common;
using OWML.Common.Menus;
using OWML.ModHelper.Events;
using UnityEngine;

namespace OWML.ModHelper.Menus
{
    public class ModMainMenu : ModMenu, IModMainMenu
    {
        public IModTabbedMenu OptionsMenu { get; }

        public IModTitleButton ResumeExpeditionButton { get; private set; }
        public IModTitleButton NewExpeditionButton { get; private set; }
        public IModTitleButton OptionsButton { get; private set; }
        public IModTitleButton ViewCreditsButton { get; private set; }
        public IModTitleButton SwitchProfileButton { get; private set; }
        public IModTitleButton QuitButton { get; private set; }

        private TitleAnimationController _anim;

        public ModMainMenu(IModConsole console) : base(console)
        {
            OptionsMenu = new ModOptionsMenu(console);
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
            InvokeOnInit();
        }

        public override IBaseButton AddButton(IBaseButton button, int index)
        {
            var modButton = base.AddButton(button, index);
            var fadeControllers = TitleButtons.OrderBy(x => x.Index).Select(x => new CanvasGroupFadeController
            {
                group = x.Button.GetComponent<CanvasGroup>()
            });
            _anim.SetValue("_buttonFadeControllers", fadeControllers.ToArray());
            return modButton;
        }

    }
}
