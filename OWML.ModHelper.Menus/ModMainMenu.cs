using System.Linq;
using OWML.Common;
using OWML.Common.Menus;
using OWML.ModHelper.Events;
using UnityEngine;

namespace OWML.ModHelper.Menus
{
    public class ModMainMenu : ModMenu, IModMainMenu
    {
        public IModTabbedMenu OptionsMenu { get; } = new ModOptionsMenu();

        public IModButton ResumeExpeditionButton { get; private set; }
        public IModButton NewExpeditionButton { get; private set; }
        public IModButton OptionsButton { get; private set; }
        public IModButton ViewCreditsButton { get; private set; }
        public IModButton SwitchProfileButton { get; private set; }
        public IModButton QuitButton { get; private set; }

        private TitleAnimationController _anim;

        public void Initialize(TitleScreenManager titleScreenManager)
        {
            _anim = titleScreenManager.GetComponent<TitleAnimationController>();
            var menu = titleScreenManager.GetValue<Menu>("_mainMenu");
            Initialize(menu);

            ResumeExpeditionButton = GetTitleButton("Button-ResumeGame");
            NewExpeditionButton = GetTitleButton("Button-NewGame");
            OptionsButton = GetTitleButton("Button-Options");
            ViewCreditsButton = GetTitleButton("Button-Credits");
            SwitchProfileButton = GetTitleButton("Button-Profile");
            QuitButton = GetTitleButton("Button-Exit");

            var tabbedMenu = titleScreenManager.GetValue<TabbedMenu>("_optionsMenu");
            OptionsMenu.Initialize(tabbedMenu);
            InvokeOnInit();
        }

        public override IModButtonBase AddButton(IModButtonBase button, int index)
        {
            var modButton = base.AddButton(button, index);
            var fadeControllers = Buttons.OrderBy(x => x.Index).Select(x => new CanvasGroupFadeController
            {
                group = x.Button.GetComponent<CanvasGroup>()
            });
            _anim.SetValue("_buttonFadeControllers", fadeControllers.ToArray());
            return modButton;
        }

    }
}
