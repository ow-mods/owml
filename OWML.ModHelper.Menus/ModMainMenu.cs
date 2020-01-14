using System.Linq;
using OWML.Common;
using OWML.ModHelper.Events;
using UnityEngine;

namespace OWML.ModHelper.Menus
{
    public class ModMainMenu : ModMenu
    {
        private readonly TitleAnimationController _anim;
        
        public ModMainMenu(IModLogger logger, IModConsole console) : base(logger, console)
        {
            var titleScreenManager = GameObject.FindObjectOfType<TitleScreenManager>();
            _anim = titleScreenManager.GetComponent<TitleAnimationController>();
            var menu = titleScreenManager.GetValue<Menu>("_mainMenu");
            Initialize(menu);
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
