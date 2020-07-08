using System;
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
        public ILayoutManager Layout { get; }

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
            var scale = button.transform.localScale;
            Button = button;
            Button.onClick.AddListener(() => OnClick?.Invoke());
            GameObject.Destroy(Button.GetComponentInChildren<Text>(true).gameObject);
            var layoutObject = new GameObject("LayoutGroup", typeof(RectTransform));
            layoutObject.transform.SetParent(button.transform);
            var target = layoutObject.AddComponent<Image>();
            target.raycastTarget = true;
            target.color = Color.clear;
            var layoutGroup = layoutObject.AddComponent<HorizontalLayoutGroup>();
            Initialize(menu);
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = false;
            var styleManager = GameObject.FindObjectOfType<UIStyleManager>();
            var styleApplier = ModUIStyleApplier.ReplaceStyleApplier(Button.gameObject);
            Layout = new LayoutManager(layoutGroup, styleManager, styleApplier, scale);
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
