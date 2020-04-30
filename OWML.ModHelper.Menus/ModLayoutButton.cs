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
            var temp = Button.GetComponentInChildren<Text>().gameObject;
            GameObject.Destroy(temp);
            Button.transform.GetChild(1).gameObject.SetActive(false);
            temp = new GameObject("LayoutGroup", new Type[] { typeof(RectTransform) });
            temp.transform.SetParent(button.transform);
            var img = temp.AddComponent<Image>();
            img.raycastTarget = true;
            img.color = new Color(255, 255, 255, 0);
            LayoutGroup = temp.AddComponent<HorizontalLayoutGroup>();
            Initialize(menu);
            this._buttonStyleApplier = Button.GetComponent<UIStyleApplier>();
            LayoutGroup.childControlWidth = false;
            LayoutGroup.childControlHeight = false;
            LayoutGroup.childForceExpandHeight = false;
            LayoutGroup.childForceExpandWidth = false;
            LayoutGroup.childAlignment = TextAnchor.MiddleCenter;
            UpdateState();
        }
        public void UpdateState()
        {
            LayoutGroup.transform.localPosition = Button.transform.GetChild(0).localPosition;
            ((RectTransform)LayoutGroup.transform).pivot = ((RectTransform)Button.transform.GetChild(0)).pivot;
            FieldInfo texts = typeof(UIStyleApplier).GetField("_textItems", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo foregrounds = typeof(UIStyleApplier).GetField("_foregroundGraphics", BindingFlags.NonPublic | BindingFlags.Instance);    
            Text[] temp = Button.gameObject.GetComponentsInChildren<Text>();
            texts.SetValue(_buttonStyleApplier, temp);
            Graphic[] temp2 = temp;
            foregrounds.SetValue(_buttonStyleApplier, temp2);
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
