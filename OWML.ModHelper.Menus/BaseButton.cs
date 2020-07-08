using System;
using OWML.Common.Menus;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public abstract class BaseButton : IBaseButton
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

        protected BaseButton(Button button, IModMenu menu)
        {
            Button = button;
            Button.onClick.AddListener(() => OnClick?.Invoke());
            Initialize(menu);
        }

        public void Initialize(IModMenu menu)
        {
            Menu = menu;
        }

        public IModButton Copy()
        {
            var button = GameObject.Instantiate(Button);
            GameObject.Destroy(button.GetComponent<SubmitAction>());
            return new ModButton(button, Menu)
            {
                Index = Index + 1
            };
        }

        public IModButton Copy(string title)
        {
            var copy = Copy();
            copy.Title = title;
            return copy;
        }

        public IModButton Copy(int index)
        {
            var copy = Copy();
            copy.Index = index;
            return copy;
        }

        public IModButton Copy(string title, int index)
        {
            var copy = Copy(title);
            copy.Index = index;
            return copy;
        }

        public IModButton Duplicate()
        {
            var copy = Copy();
            Menu.AddButton(copy);
            return copy;
        }

        public IModButton Duplicate(string title)
        {
            var dupe = Duplicate();
            dupe.Title = title;
            return dupe;
        }

        public IModButton Duplicate(int index)
        {
            var dupe = Duplicate();
            dupe.Index = index;
            return dupe;
        }

        public IModButton Duplicate(string title, int index)
        {
            var dupe = Duplicate(title);
            dupe.Index = index;
            return dupe;
        }

        public IModButton Replace()
        {
            var duplicate = Duplicate();
            Hide();
            return duplicate;
        }

        public IModButton Replace(string title)
        {
            var replacement = Replace();
            replacement.Title = title;
            return replacement;
        }

        public IModButton Replace(int index)
        {
            var replacement = Replace();
            replacement.Index = index;
            return replacement;
        }

        public IModButton Replace(string title, int index)
        {
            var replacement = Replace(title);
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

        public void SetControllerCommand(SingleAxisCommand inputCommand)
        {
            Button.gameObject.AddComponent<ControllerButton>().Init(inputCommand);
        }

    }
}
