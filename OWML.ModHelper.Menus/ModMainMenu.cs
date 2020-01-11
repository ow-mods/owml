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
        private readonly IModLogger _logger;
        private readonly IModConsole _console;
        private readonly TitleAnimationController _anim;
        private readonly List<CanvasGroupFadeController> _fadeControllers;
        private readonly Menu _menu;

        public ModMainMenu(IModLogger logger, IModConsole console)
        {
            _logger = logger;
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

        public Button AddButton(string title, int index)
        {
            _console.WriteLine("Adding main menu button: " + title);

            var original = _menu.GetComponentInChildren<Button>();
            _logger.Log("Copying button: " + original.name);

            var copy = GameObject.Instantiate(original, _menu.transform);
            copy.name = title;
            copy.transform.SetSiblingIndex(index + 2);

            GameObject.Destroy(copy.GetComponentInChildren<LocalizedText>());
            GameObject.Destroy(copy.GetComponent<SubmitAction>());

            copy.GetComponentInChildren<Text>().text = title;

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
