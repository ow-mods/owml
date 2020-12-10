using System;
using OWML.Common.Enums;
using OWML.Common.Interfaces.Menus;
using OWML.Logging;
using OWML.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModPopupMenu : ModMenu, IModPopupMenu
    {
        public event Action OnOpened;

        public event Action OnClosed;

        [Obsolete("Use OnOpened instead.")]
        public Action OnOpen { get; set; }

        [Obsolete("Use OnClosed instead.")]
        public Action OnClose { get; set; }

        public bool IsOpen { get; private set; }

        private Text _title;
        public string Title
        {
            get => _title.text;
            set => _title.text = value;
        }

        public override void Initialize(Menu menu, LayoutGroup layoutGroup)
        {
            base.Initialize(menu, layoutGroup);
            _title = Menu.GetComponentInChildren<Text>(true);
            var localizedText = _title.GetComponent<LocalizedText>();
            if (localizedText != null)
            {
                Title = UITextLibrary.GetString(localizedText.GetValue<UITextType>("_textID"));
                GameObject.Destroy(localizedText);
            }
            Menu.OnActivateMenu += OnActivateMenu;
            Menu.OnDeactivateMenu += OnDeactivateMenu;
        }

        private void OnDeactivateMenu()
        {
            IsOpen = false;
            OnClose?.Invoke();
            OnClosed?.Invoke();
        }

        private void OnActivateMenu()
        {
            IsOpen = true;
            OnOpen?.Invoke();
            OnOpened?.Invoke();
        }

        public virtual void Open()
        {
            if (Menu == null)
            {
                ModConsole.OwmlConsole.WriteLine("Warning - Can't open menu, it doesn't exist.", MessageType.Warning);
                return;
            }
            SelectFirst();
            Menu.EnableMenu(true);
        }

        public void Close()
        {
            if (Menu == null)
            {
                ModConsole.OwmlConsole.WriteLine("Warning - Can't close menu, it doesn't exist.", MessageType.Warning);
                return;
            }
            Menu.EnableMenu(false);
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
                ModConsole.OwmlConsole.WriteLine("Warning - Can't copy menu, it doesn't exist.", MessageType.Warning);
                return null;
            }
            var menu = GameObject.Instantiate(Menu, Menu.transform.parent);
            var modMenu = new ModPopupMenu();
            modMenu.Initialize(menu);
            return modMenu;
        }

        public IModPopupMenu Copy(string title)
        {
            var copy = Copy();
            copy.Title = title;
            return copy;
        }

    }
}
