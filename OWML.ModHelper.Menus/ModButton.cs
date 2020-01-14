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

        public int Index
        {
            get => Button.transform.GetSiblingIndex() - GetButtonOffset();
            set => Button.transform.SetSiblingIndex(value + GetButtonOffset());
        }

        public Button Button { get; }

        private readonly Text _text; 

        public ModButton(Button button)
        {
            Button = button;
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
            return new ModButton(button);
        }

        public void Show()
        {
            Button.gameObject.SetActive(true);
        }

        public void Hide()
        {
            Button.gameObject.SetActive(false);
        }

        private int GetButtonOffset()
        {
            if (Button == null)
            {
                return 0;
            }
            var parent = Button.transform.parent;
            if (parent == null)
            {
                return 0;
            }
            var firstButton = parent.GetComponentInChildren<Button>();
            return firstButton == null ? 0 : firstButton.transform.GetSiblingIndex();
        }

    }
}
