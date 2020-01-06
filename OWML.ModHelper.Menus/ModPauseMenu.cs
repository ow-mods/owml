using System;
using System.Collections.Generic;
using System.Linq;
using OWML.Common;
using OWML.ModHelper.Events;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModPauseMenu : IModPopupMenu
    {
        public Action OnOpen { get; set; }
        public Action OnClose { get; set; }
        public bool IsOpen { get; private set; }

        private readonly IModLogger _logger;
        private readonly IModConsole _console;
        private PauseMenuManager _pauseMenuManager;
        private Menu _menu;
        private LayoutGroup _layoutGroup;

        public ModPauseMenu(IModLogger logger, IModConsole console)
        {
            _logger = logger;
            _console = console;
        }

        public List<Button> GetButtons()
        {
            if (_menu == null)
            {
                _console.WriteLine("Warning: can't get pause buttons before player wakes up");
                return new List<Button>();
            }
            return _layoutGroup.GetComponentsInChildren<Button>().ToList();
        }

        public Button AddButton(string name, int index)
        {
            _console.WriteLine("Adding pause button: " + name);

            if (_menu == null)
            {
                Initialize();
            }

            if (_menu == null)
            {
                _console.WriteLine("Warning: can't add pause buttons before player wakes up.");
                return null;
            }

            var original = _layoutGroup.GetComponentInChildren<Button>();
            _logger.Log("Copying button: " + original.name);

            var copy = GameObject.Instantiate(original, _layoutGroup.transform);
            copy.name = name;
            copy.transform.SetSiblingIndex(index + 2);

            GameObject.Destroy(copy.GetComponentInChildren<LocalizedText>());
            GameObject.Destroy(copy.GetComponent<SubmitAction>());

            copy.GetComponentInChildren<Text>().text = name;

            return copy;
        }

        private void Initialize()
        {
            _logger.Log("Trying to initialize pause menu");
            _pauseMenuManager = GameObject.FindObjectOfType<PauseMenuManager>();
            if (_pauseMenuManager == null)
            {
                _console.WriteLine("Warning: can't initialize pause menu before player wakes up");
                return;
            }
            _menu = _pauseMenuManager.GetValue<Menu>("_pauseMenu");
            _layoutGroup = _menu.GetComponentInChildren<LayoutGroup>();
            _menu.OnActivateMenu += OnActivateMenu;
            _menu.OnDeactivateMenu += OnDeactivateMenu;
        }

        private void OnDeactivateMenu()
        {
            IsOpen = false;
            OnClose?.Invoke();
        }

        private void OnActivateMenu()
        {
            IsOpen = true;
            OnOpen?.Invoke();
        }

        public void Open()
        {
            if (_menu == null)
            {
                Console.WriteLine("Warning: can't open menu, it doesn't exist.");
                return;
            }
            _pauseMenuManager.TryOpenPauseMenu();
        }

        public void Close()
        {
            if (_menu == null)
            {
                Console.WriteLine("Warning: can't close menu, it doesn't exist.");
                return;
            }
            _menu.Deactivate();
        }

        public void Toggle()
        {
            if (IsOpen)
            {
                Close();
            }
            else
            {
                Open();
            }
        }

    }
}
