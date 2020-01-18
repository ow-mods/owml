using System;
using OWML.Common.Menus;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModButton : IModButton
    {
        public event Action OnClick;

        public Button Button { get; private set; }

        public IModMenu Menu { get; private set; }

        private Text _text;
        public string Title
        {
            get => _text.text;
            set => _text.text = value;
        }

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

        public ModButton(Button button, IModMenu menu)
        {
            Button = button;
            Button.onClick.AddListener(() =>
            {
                OnClick?.Invoke();
            });
            _text = Button.GetComponentInChildren<Text>();
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
            GameObject.Destroy(button.GetComponentInChildren<LocalizedText>());
            return new ModButton(button, Menu)
            {
                Index = Index + 1
            };
        }

        public IModButton Duplicate()
        {
            var copy = Copy();
            Menu.AddButton(copy);
            return copy;
        }

        public IModButton Replace()
        {
            var duplicate = Duplicate();
            Hide();
            return duplicate;
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
