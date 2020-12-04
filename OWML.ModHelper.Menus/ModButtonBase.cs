using System;
using OWML.Common.Interfaces.Menus;
using OWML.ModHelper.Events;
using Object = UnityEngine.Object;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public abstract class ModButtonBase : IModButtonBase
    {
        public event Action OnClick;

        public Button Button { get; }
        public IModMenu Menu { get; private set; }

        private int _index;
        public int Index
        {
            get => Button.transform.parent == null ? _index : Button.transform.GetSiblingIndex();
            set
            {
                _index = value;
                Button.transform.SetSiblingIndex(value);
            }
        }
        public bool IsSelected => _uIStyleApplier?.GetValue<bool>("_selected") ?? false;

        private readonly UIStyleApplier _uIStyleApplier;

        protected ModButtonBase(Button button, IModMenu menu)
        {
            _uIStyleApplier = button.GetComponent<UIStyleApplier>();
            Button = button;
            Button.onClick.AddListener(() => OnClick?.Invoke());
            Initialize(menu);
        }

        public IModButtonBase Copy()
        {
            var button = Object.Instantiate(Button);
            Object.Destroy(button.GetComponent<SubmitAction>());
            var modButton = (IModButtonBase)Activator.CreateInstance(GetType(), button, Menu);
            modButton.Index = Index + 1;
            return modButton;
        }

        public void Initialize(IModMenu menu)
        {
            Menu = menu;
        }

        public IModButtonBase Copy(int index)
        {
            var copy = Copy();
            copy.Index = index;
            return copy;
        }

        public IModButtonBase Duplicate()
        {
            var copy = Copy();
            Menu.AddButton(copy);
            return copy;
        }

        public IModButtonBase Duplicate(int index)
        {
            var dupe = Duplicate();
            dupe.Index = index;
            return dupe;
        }

        public IModButtonBase Replace()
        {
            var duplicate = Duplicate();
            Hide();
            return duplicate;
        }

        public IModButtonBase Replace(int index)
        {
            var replacement = Replace();
            replacement.Index = index;
            return replacement;
        }

        public void Show()
        {
            Button.gameObject.SetActive(true);
        }

        public void Hide()
        {
            Button.gameObject.SetActive(false);
        }
        
    }
}
