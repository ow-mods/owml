using System;
using System.Reflection;
using OWML.Common.Menus;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModLayoutButton : IModLayoutButton
    {
        public event Action OnClick;
        public Button Button { get; }
        public IModMenu Menu { get; private set; }
        public HorizontalLayoutGroup LayoutGroup { get; private set; }

        private int _index;
        private readonly UIStyleApplier _buttonStyleApplier;
        private readonly FieldInfo _texts;
        private readonly FieldInfo _foregrounds;

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
            target.color = new Color(255, 255, 255, 0);//transparent
            LayoutGroup = layoutObject.AddComponent<HorizontalLayoutGroup>();
            Initialize(menu);
            _buttonStyleApplier = Button.GetComponent<UIStyleApplier>();
            LayoutGroup.childControlWidth = false;
            LayoutGroup.childControlHeight = false;
            LayoutGroup.childForceExpandHeight = false;
            LayoutGroup.childForceExpandWidth = false;
            LayoutGroup.childAlignment = TextAnchor.MiddleCenter;
            LayoutGroup.transform.localPosition = Vector3.zero;
            ((RectTransform)LayoutGroup.transform).pivot = new Vector2(0.5f, 0.5f);//center
            _texts = typeof(UIStyleApplier).GetField("_textItems", BindingFlags.NonPublic | BindingFlags.Instance);
            _foregrounds = typeof(UIStyleApplier).GetField("_foregroundGraphics", BindingFlags.NonPublic | BindingFlags.Instance);
            UpdateState();
        }

        public void UpdateState()
        {
            var currentTexts = Button.gameObject.GetComponentsInChildren<Text>();
            _texts.SetValue(_buttonStyleApplier, currentTexts);
            _foregrounds.SetValue(_buttonStyleApplier, currentTexts);
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
