using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OWML.Common;
using OWML.Common.Menus;
using OWML.ModHelper.Events;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModPopupMenu : ModMenu, IModPopupMenu
    {
        public Action OnOpen { get; set; }
        public Action OnClose { get; set; }

        public bool IsOpen { get; private set; }

        private Text _title;
        public string Title
        {
            get => _title.text;
            set => _title.text = value;
        }

        public List<ModInput<object>> InputElements { get; private set; }

        private readonly IModLogger _logger;
        private readonly IModConsole _console;

        public ModPopupMenu(IModLogger logger, IModConsole console) : base(logger, console)
        {
            _logger = logger;
            _console = console;
        }

        public override void Initialize(Menu menu)
        {
            base.Initialize(menu);
            _title = Menu.GetComponentInChildren<Text>();
            var localizedText = _title.GetComponent<LocalizedText>();
            if (localizedText != null)
            {
                Title = UITextLibrary.GetString(localizedText.GetValue<UITextType>("_textID"));
                GameObject.Destroy(localizedText);
            }
            Menu.OnActivateMenu += OnActivateMenu;
            Menu.OnDeactivateMenu += OnDeactivateMenu;

            InputElements = new List<ModInput<object>>();
            var toggleElements = Menu.GetComponentsInChildren<TwoButtonToggleElement>();
            foreach (var toggleElement in toggleElements)
            {
                var lol = new ModToggleInput(toggleElement);
                InputElements.Add(lol);
            }
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

        public void Close()
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

        public IModPopupMenu Copy()
        {
            if (Menu == null)
            {
                _console.WriteLine("Warning: can't copy menu, it doesn't exist.");
                return null;
            }
            var menu = GameObject.Instantiate(Menu, Menu.transform.parent);
            var modMenu = new ModPopupMenu(_logger, _console);
            modMenu.Initialize(menu);
            return modMenu;
        }

        public IModPopupMenu Copy(string title)
        {
            var copy = Copy();
            copy.Title = title;
            return copy;
        }

        [Obsolete("Use Copy instead")]
        public IModPopupMenu CreateCopy(string title)
        {
            if (Menu == null)
            {
                _console.WriteLine("Warning: can't copy menu, it doesn't exist.");
                return null;
            }
            var menu = Copy();
            menu.Title = title;
            return menu;
        }

    }
}
