using System.Collections.Generic;
using System.Linq;
using OWML.Common;
using OWML.ModHelper.Events;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModMainMenu : IModMenu
    {
        private readonly IModConsole _console;
        private readonly TitleAnimationController _anim;
        private readonly List<CanvasGroupFadeController> _fadeControllers;
        private readonly Menu _menu;

        public ModMainMenu(IModConsole console)
        {
            _console = console;
            var titleScreenManager = GameObject.FindObjectOfType<TitleScreenManager>();
            _anim = titleScreenManager.GetComponent<TitleAnimationController>();
            _menu = titleScreenManager.GetValue<Menu>("_mainMenu");
            _fadeControllers = _anim.GetValue<CanvasGroupFadeController[]>("_buttonFadeControllers").ToList();
        }

        public List<Button> GetButtons()
        {
            return _menu.GetComponentsInChildren<Button>().ToList();
        }

        public Button AddButton(string name, int index)
        {
            _console.WriteLine("Adding main menu button: " + name);

            var original = _menu.GetComponentInChildren<Button>();
            _console.WriteLine("Copying button: " + original.name);

            var copy = GameObject.Instantiate(original, _menu.transform);
            copy.name = name;
            copy.transform.SetSiblingIndex(index);

            GameObject.Destroy(copy.GetComponentInChildren<LocalizedText>());
            GameObject.Destroy(copy.GetComponent<SubmitAction>());

            copy.GetComponentInChildren<Text>().text = name;

            var fadeController = new CanvasGroupFadeController 
            {
                group = copy.GetComponent<CanvasGroup>()
            };
            _fadeControllers.Insert(index, fadeController);
            _anim.SetValue("_buttonFadeControllers", _fadeControllers.ToArray());
            
            return copy;
        }

    }
}
