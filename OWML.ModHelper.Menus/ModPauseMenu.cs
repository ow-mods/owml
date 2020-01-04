using System.Collections.Generic;
using System.Linq;
using OWML.Common;
using OWML.ModHelper.Events;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModPauseMenu : IModMenu
    {
        private readonly IModConsole _console;
        private Menu _menu;
        private LayoutGroup _layoutGroup;
        private bool _isInitialized;

        public ModPauseMenu(IModConsole console)
        {
            _console = console;
        }

        public List<Button> GetButtons()
        {
            if (!_isInitialized)
            {
                _console.WriteLine("Warning: can't get pause buttons before player wakes up");
                return new List<Button>();
            }
            return _layoutGroup.GetComponentsInChildren<Button>().ToList();
        }

        public Button AddButton(string name, int index)
        {
            _console.WriteLine("Adding pause button: " + name);

            if (!_isInitialized)
            {
                Initialize();
            }

            if (!_isInitialized)
            {
                _console.WriteLine("Warning: can't add pause buttons before player wakes up.");
                return null;
            }

            var original = _layoutGroup.GetComponentInChildren<Button>();
            _console.WriteLine("Copying button: " + original.name);

            var copy = GameObject.Instantiate(original, _layoutGroup.transform);
            copy.name = name;
            copy.transform.SetSiblingIndex(index);

            GameObject.Destroy(copy.GetComponentInChildren<LocalizedText>());
            GameObject.Destroy(copy.GetComponent<SubmitAction>());

            copy.GetComponentInChildren<Text>().text = name;

            return copy;
        }

        private void Initialize()
        {
            _console.WriteLine("Trying to initialize pause menu");
            var pauseMenuManager = GameObject.FindObjectOfType<PauseMenuManager>();
            if (pauseMenuManager == null)
            {
                _console.WriteLine("Warning: can't initialize pause menu before player wakes up");
                return;
            }
            _menu = pauseMenuManager.GetValue<Menu>("_pauseMenu");
            _layoutGroup = _menu.GetComponentInChildren<LayoutGroup>();
            _isInitialized = true;
        }

    }
}
