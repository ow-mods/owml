using OWML.Common.Menus;
using OWML.ModHelper.Events;
using OWML.Common;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace OWML.ModHelper.Menus
{
    public class ModInputInput : ModInput<string>, IModInputInput
    {

        private string _value;
        private Vector3 scale;
        private UIStyleManager styleManager;
        private HorizontalLayoutGroup _layoutGroup;
        public IModLayoutButton Button { get; }
        protected readonly IModInputMenu InputMenu;
        protected readonly TwoButtonToggleElement ToggleElement;

        public ModInputInput(TwoButtonToggleElement element, IModMenu menu, IModInputMenu inputMenu) : base(element, menu)
        {
            ToggleElement = element;
            InputMenu = inputMenu;
            OnChange += Upd;
            scale = element.GetValue<Button>("_buttonTrue").transform.localScale;
            Button = new ModLayoutButton(element.GetValue<Button>("_buttonTrue"), menu);
            Button.OnClick += Open;
            var noButton = ToggleElement.GetValue<Button>("_buttonFalse");
            noButton.transform.parent.gameObject.SetActive(false);
            _layoutGroup = Button.LayoutGroup;
            ((RectTransform)_layoutGroup.transform).sizeDelta = new Vector2(((RectTransform)Button.Button.transform.parent).sizeDelta.x * 2, ((RectTransform)Button.Button.transform.parent).sizeDelta.y);
            styleManager = MonoBehaviour.FindObjectOfType<UIStyleManager>();

            var layoutGroup = Button.Button.transform.parent.parent.GetComponent<HorizontalLayoutGroup>();
            layoutGroup.childControlWidth = true;
            layoutGroup.childForceExpandWidth = true;
            Button.Button.transform.parent.GetComponent<LayoutElement>().preferredWidth = 100;
        }

        private void Upd(string thing)
        {
            int cnt = _layoutGroup.transform.childCount;
            for (int i = cnt - 1; i >= 0; i--)
                GameObject.Destroy(_layoutGroup.transform.GetChild(i).gameObject);
            string[] str = thing.Split('/');
            for (int i = 0; i < str.Length; i++)
			{
                string[] st = str[i].Split('+');
                for (int j = 0; j < st.Length; j++)
				{
                    Texture2D tex;
                    if (st[j].Contains("Xbox_"))
                    {
                        tex = InputTranslator.GetButtonTexture((XboxButton)Enum.Parse(typeof(XboxButton), st[j].Substring(5)));
                    }
                    else
                    {
                        tex = InputTranslator.GetButtonTexture((KeyCode)Enum.Parse(typeof(KeyCode), st[j]));
                    }
                    Sprite spr = Sprite.Create(tex, new Rect(0f, 0f, (float)tex.width, (float)tex.height), new Vector2(0.5f, 0.5f));
                    GameObject gameObject = new GameObject("ButtonImage", new Type[] { typeof(RectTransform) });
                    Image pic = gameObject.AddComponent<Image>();
                    pic.sprite = spr;
                    pic.SetLayoutDirty();
                    gameObject.AddComponent<LayoutElement>();
                    gameObject.transform.SetParent(_layoutGroup.transform);
                    gameObject.transform.localScale = scale;
                    ((RectTransform)gameObject.transform).sizeDelta = new Vector2((float)tex.width*0.75f, (float)tex.height * 0.75f);
                    ((RectTransform)gameObject.transform).pivot = new Vector2(0.5f, 0.5f);
                    if (j<st.Length-1)
                        AddText("+");
                }
                if (i < str.Length - 1)
                    AddText("/");
            }
            Button.UpdateState();
		}
        private void AddText(string txt)
		{
            GameObject gameObject = new GameObject("Text", new Type[] { typeof(RectTransform) });
            Text text = gameObject.AddComponent<Text>();
            text.text = txt;
            text.fontSize = 36;
            text.font = styleManager.GetMenuFont();
            text.color = styleManager.GetButtonForegroundMenuColor(UIElementState.NORMAL);
            text.alignment = TextAnchor.MiddleCenter;
            gameObject.AddComponent<LayoutElement>();
            gameObject.transform.SetParent(_layoutGroup.transform);
            gameObject.transform.localScale = scale;
            ((RectTransform)gameObject.transform).sizeDelta = new Vector2(text.preferredWidth, ((RectTransform)gameObject.transform).sizeDelta.y * 0.75f);
            ((RectTransform)gameObject.transform).pivot = new Vector2(0.5f, 0.5f);
        }
        protected void Open()
        {
            InputMenu.OnConfirm += OnConfirm;
            InputMenu.OnCancel += OnCancel;
            InputMenu.Open(InputType.Text, _value);
        }

        private void OnConfirm(string text)
        {
            OnCancel();
            Value = text;
        }

        private void OnCancel()
        {
            InputMenu.OnConfirm -= OnConfirm;
            InputMenu.OnCancel -= OnCancel;
        }

        public override string Value
        {
            get => _value;
            set
            {
                _value = value;
                InvokeOnChange(value);
            }
        }

        public IModInputInput Copy()
        {
            var copy = GameObject.Instantiate(ToggleElement);
            GameObject.Destroy(copy.GetComponentInChildren<LocalizedText>());
            return new ModInputInput(copy, Menu, InputMenu);
        }

        public IModInputInput Copy(string title)
        {
            var copy = Copy();
            copy.Title = title;
            return copy;
        }

    }
}
