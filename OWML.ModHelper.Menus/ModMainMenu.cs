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

        public override IModButton AddButton(IModButton button, int index)
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
