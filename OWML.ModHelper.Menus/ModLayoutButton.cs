using System;
using System.Reflection;
using OWML.Common.Menus;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace OWML.ModHelper.Menus
{
    public class ModLayoutButton : IModLayoutButton
    {
        public event Action OnClick;

        public Button Button { get; }
        public IModMenu Menu { get; private set; }
        public HorizontalLayoutGroup LayoutGroup { get; private set; }
        private UIStyleApplier _buttonStyleApplier;
        FieldInfo texts, foregrounds;

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
        public ModLayoutButton(Button button, IModMenu menu)
        {
            Button = button;
            Button.onClick.AddListener(() => OnClick?.Invoke());
            GameObject.Destroy(Button.GetComponentInChildren<Text>().gameObject);
            var layoutObject = new GameObject("LayoutGroup", new Type[] { typeof(RectTransform) });
            layoutObject.transform.SetParent(button.transform);
            var target = layoutObject.AddComponent<Image>();
            target.raycastTarget = true;
            target.color = new Color(255, 255, 255, 0);
            LayoutGroup = layoutObject.AddComponent<HorizontalLayoutGroup>();
            Initialize(menu);
            this._buttonStyleApplier = Button.GetComponent<UIStyleApplier>();
            LayoutGroup.childControlWidth = false;
            LayoutGroup.childControlHeight = false;
            LayoutGroup.childForceExpandHeight = false;
            LayoutGroup.childForceExpandWidth = false;
            LayoutGroup.childAlignment = TextAnchor.MiddleCenter;
            LayoutGroup.transform.localPosition = Vector3.zero;
            ((RectTransform)LayoutGroup.transform).pivot = new Vector2(0.5f, 0.5f);
            texts = typeof(UIStyleApplier).GetField("_textItems", BindingFlags.NonPublic | BindingFlags.Instance);
            foregrounds = typeof(UIStyleApplier).GetField("_foregroundGraphics", BindingFlags.NonPublic | BindingFlags.Instance);
            UpdateState();
        }
        public void UpdateState()
        {
            Text[] currentTexts = Button.gameObject.GetComponentsInChildren<Text>();
            texts.SetValue(_buttonStyleApplier, currentTexts);
            foregrounds.SetValue(_buttonStyleApplier, (Graphic[])currentTexts);
        }
        public void Initialize(IModMenu menu)
        {
            Menu = menu;
        }

        public IModLayoutButton Copy()
        {
            var button = GameObject.Instantiate(Button);
            GameObject.Destroy(button.GetComponent<SubmitAction>());
            return new ModLayoutButton(button, Menu)
            {
                Index = Index + 1
            };
        }

        public IModLayoutButton Copy(int index)
        {
            var copy = Copy();
            copy.Index = index;
            return copy;
        }

        public IModLayoutButton Duplicate()
        {
            var copy = Copy();
            Menu.AddLayoutButton(copy);
            return copy;
        }

        public IModLayoutButton Duplicate(int index)
        {
            var dupe = Duplicate();
            dupe.Index = index;
            return dupe;
        }

        public IModLayoutButton Replace()
        {
            var duplicate = Duplicate();
            Hide();
            return duplicate;
        }

        public IModLayoutButton Replace(int index)
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
