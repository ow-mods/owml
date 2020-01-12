using System;
using OWML.Common;
using OWML.ModHelper.Events;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModButton : IModButton
    {
        public Action OnClick { get; set; }

        public string Title
        {
            get => _text.text;
            set => _text.text = value;
        }

        private int _index;
        public int Index
        {
            get => _index;
            set
            {
                _index = value;
                Button.transform.SetSiblingIndex(value + _menu?.ButtonOffset ?? 0);
            }
        }

        public Button Button { get; }

        private readonly Text _text;
        private readonly IModMenu _menu;

        public ModButton(Button button, IModMenu menu)
        {
            Button = button;
            _menu = menu;
            Button.onClick.AddListener(() =>
            {
                OnClick?.Invoke();
            });
            _text = Button.GetComponentInChildren<Text>();
            var localizedText = _text.GetComponent<LocalizedText>();
            if (localizedText != null)
            {
                Title = UITextLibrary.GetString(localizedText.GetValue<UITextType>("_textID"));
                GameObject.Destroy(localizedText);
            }
        }

        public IModButton Copy()
        {
            var button = GameObject.Instantiate(Button);
            GameObject.Destroy(button.GetComponent<SubmitAction>());
            return new ModButton(button, _menu)
            {
                Index = Index
            };
        }

        public IModButton Duplicate()
        {
            var copy = Copy();
            copy.SetValue("_menu", _menu);
            _menu.AddButton(copy);
            return copy;
        }

        public IModButton Replace()
        {
            var dupe = Duplicate();
            Button.gameObject.SetActive(false);
            return dupe;
        }

    }
}
