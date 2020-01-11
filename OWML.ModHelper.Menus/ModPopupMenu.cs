using System;
using System.Collections.Generic;
using System.Linq;
using OWML.Common;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModPopupMenu : IModPopupMenu
    {
        public Action OnOpen { get; set; }
        public Action OnClose { get; set; }
        public Action OnInit { get; set; }

        public bool IsOpen { get; private set; }

        private readonly IModLogger _logger;
        private readonly IModConsole _console;

        protected Menu Menu;
        private LayoutGroup _layoutGroup;
        private Text _title;

        public ModPopupMenu(IModLogger logger, IModConsole console)
        {
            _logger = logger;
            _console = console;
        }

        public virtual List<Button> GetButtons()
        {
            if (Menu == null)
            {
                _console.WriteLine("Warning: can't get buttons before menu exists");
                return new List<Button>();
            }
            return _layoutGroup.GetComponentsInChildren<Button>().ToList();
        }

        public virtual Button AddButton(string name, int index)
        {
            if (Menu == null)
            {
                _console.WriteLine("Warning: can't add button before menu exists");
                return null;
            }

            _console.WriteLine("Adding button: " + name);

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

        public void Initialize(Menu menu)
        {
            Menu = menu;
            _layoutGroup = Menu.GetComponentInChildren<LayoutGroup>();
            _title = Menu.GetComponentInChildren<Text>();
            GameObject.Destroy(_title.GetComponent<LocalizedText>());
            Menu.OnActivateMenu += OnActivateMenu;
            Menu.OnDeactivateMenu += OnDeactivateMenu;
            OnInit?.Invoke();
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

        public virtual void Open()
        {
            if (Menu == null)
            {
                _console.WriteLine("Warning: can't open menu, it doesn't exist.");
                return;
            }
            Menu.EnableMenu(true);
        }

        public virtual void Close()
        {
            if (Menu == null)
            {
                _console.WriteLine("Warning: can't close menu, it doesn't exist.");
                return;
            }
            Menu.Deactivate();
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

        public IModPopupMenu CreateCopy(string title)
        {
            if (Menu == null)
            {
                _console.WriteLine("Warning: can't copy menu, it doesn't exist.");
                return null;
            }
            var copy = GameObject.Instantiate(Menu, Menu.transform.parent);
            var modMenu = new ModPopupMenu(_logger, _console);
            modMenu.Initialize(copy);
            modMenu.SetTitle(title);
            return modMenu;
        }

        private void SetTitle(string title)
        {
            _title.text = title;
        }

    }
}
