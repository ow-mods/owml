using System.Collections.Generic;
using System.Linq;
using OWML.Common;
using OWML.ModHelper.Events;
using UnityEngine;

namespace OWML.ModHelper.Menus
{
    public class ModMainMenu : ModMenu
    {
        private readonly IModLogger _logger;
        private readonly IModConsole _console;
        private readonly TitleAnimationController _anim;
        private readonly List<CanvasGroupFadeController> _fadeControllers;
        
        public ModMainMenu(IModLogger logger, IModConsole console) : base(logger, console)
        {
            _logger = logger;
            _console = console;
            var titleScreenManager = GameObject.FindObjectOfType<TitleScreenManager>();
            _anim = titleScreenManager.GetComponent<TitleAnimationController>();
            _fadeControllers = _anim.GetValue<CanvasGroupFadeController[]>("_buttonFadeControllers").ToList();
            var menu = titleScreenManager.GetValue<Menu>("_mainMenu");
            Initialize(menu);
        }

        public override void AddButton(IModButton button, int index)
        {
            base.AddButton(button, index);
            var fadeController = new CanvasGroupFadeController
            {
                group = button.Button.GetComponent<CanvasGroup>()
            };
            _fadeControllers.Insert(index, fadeController);
            _anim.SetValue("_buttonFadeControllers", _fadeControllers.ToArray());
        }

    }
}
