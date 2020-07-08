using System;
using OWML.Common.Menus;
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

        public abstract IBaseButton Copy();
        public abstract void AddToMenu(IBaseButton button);

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

        public IBaseButton Copy(int index)
        {
            var copy = Copy();
            copy.Index = index;
            return copy;
        }

        public IBaseButton Duplicate()
        {
            var copy = Copy();
            AddToMenu(copy);
            return copy;
        }

        public IBaseButton Duplicate(int index)
        {
            var dupe = Duplicate();
            dupe.Index = index;
            return dupe;
        }

        public IBaseButton Replace()
        {
            var duplicate = Duplicate();
            Hide();
            return duplicate;
        }

        public IBaseButton Replace(int index)
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

        public void SetControllerCommand(SingleAxisCommand inputCommand)
        {
            Button.gameObject.AddComponent<ControllerButton>().Init(inputCommand);
        }

    }
}
