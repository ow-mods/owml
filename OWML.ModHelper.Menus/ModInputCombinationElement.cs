using OWML.Common;
using OWML.Common.Menus;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Xml.Serialization;

namespace OWML.ModHelper.Menus
{
    public class ModInputCombinationElement : ModToggleInput, IModInputCombinationElement
    {
        private const float ScaleDown = 0.75f;
        private const string XboxPrefix = "xbox_";

        public ILayoutManager Layout { get; private set; }

        override public string Title
        {
            get => _combination;
            set
            {
                _combination = value;
                UpdateContents();
            }
        }

        private string _combination;
        private readonly GameObject _layoutObject;

        private static ModInputCombinationElementMenu _popupMenu;

        public ModInputCombinationElement(TwoButtonToggleElement toggle, IModMenu menu, ModInputCombinationElementMenu popupMenu, string combination = "") : base(toggle, menu)
        {
            _combination = combination;
            _layoutObject = toggle.transform.GetChild(1).GetChild(0).GetChild(1).gameObject;
            var layoutGroup = _layoutObject.GetComponent<HorizontalLayoutGroup>();
            var scale = toggle.transform.localScale;
            YesButton.Title = "Edit";
            YesButton.OnClick += () => OnEditClick();
            NoButton.Title = "Delete";
            NoButton.OnClick += () => OnDeleteClick();
            Initialize(menu);
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = false;
            var constantElements = new List<Graphic>();
            constantElements.Add(toggle.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).gameObject.GetComponent<Graphic>());
            toggle.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(1).gameObject.SetActive(false);
            constantElements.Add(toggle.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(1).gameObject.GetComponent<Graphic>());
            constantElements.Add(toggle.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(2).gameObject.GetComponent<Graphic>());
            Layout = new LayoutManager(layoutGroup, MonoBehaviour.FindObjectOfType<UIStyleManager>(),
                toggle.GetComponent<UIStyleApplier>(), scale, constantElements);
            UpdateContents();
            _popupMenu = popupMenu;
        }

        private void UpdateContents()
        {
            Layout.Clear();
            if (_combination != "")
            {
                var keyStrings = _combination.Split('+');
                for (var j = 0; j < keyStrings.Length; j++)
                {
                    AddKeySign(keyStrings[j]);
                    if (j < keyStrings.Length - 1)
                    {
                        Layout.AddTextAt("+", Layout.ChildCount - 1);
                    }
                }
            }
            Layout.UpdateState();
        }

        private Texture2D GetGamepadButtonTexture(string key)
        {
            return ButtonPromptLibrary.SharedInstance.GetButtonTexture((JoystickButton)Enum.Parse(typeof(JoystickButton), key));
        }

        private Texture2D GetXboxButtonTexture(string xboxKey)
        {
            switch (xboxKey[0])
            {
                case 'A':
                    return ButtonPromptLibrary.SharedInstance.GetButtonTexture(JoystickButton.FaceDown);
                case 'B':
                    return ButtonPromptLibrary.SharedInstance.GetButtonTexture(JoystickButton.FaceRight);
                case 'X':
                    return ButtonPromptLibrary.SharedInstance.GetButtonTexture(JoystickButton.FaceLeft);
                case 'Y':
                    return ButtonPromptLibrary.SharedInstance.GetButtonTexture(JoystickButton.FaceUp);
                default:
                    return GetGamepadButtonTexture(xboxKey);
            }
        }

        private void AddKeySign(string key)
        {
            Layout.AddPictureAt(
                key.Contains(XboxPrefix) ?
                GetXboxButtonTexture(key.Substring(XboxPrefix.Length)) :
                ButtonPromptLibrary.SharedInstance.GetButtonTexture((KeyCode)Enum.Parse(typeof(KeyCode), key))
                , Layout.ChildCount - 1, ScaleDown);
        }

        private void OnEditClick()
        {
            _popupMenu.OnConfirm += OnPopupMenuConfirm;
            _popupMenu.OnCancel += OnPopupMenuCancel;
            _popupMenu.Open(_combination);
        }

        private void OnPopupMenuCancel()
        {
            _popupMenu.OnConfirm -= OnPopupMenuConfirm;
            _popupMenu.OnCancel -= OnPopupMenuCancel;
        }

        private void OnPopupMenuConfirm(string combination)
        {
            OnPopupMenuCancel();
            _combination = combination;
            UpdateContents();
        }

        public void DestroySelf()
        {
            Layout.Clear();
            Layout.UpdateState();
            Title = "";
            Toggle.gameObject.SetActive(false);
            (Menu as IModInputCombinationMenu)?.CombinationElements.Remove(this);
        }

        private void OnDeleteClick()
        {
            DestroySelf();
        }

        public new IModInputCombinationElement Copy()
        {
            return Copy(_combination);
        }

        public new IModInputCombinationElement Copy(string combination)
        {
            var copy = GameObject.Instantiate(Toggle);
            GameObject.Destroy(copy.GetComponentInChildren<LocalizedText>());
            return new ModInputCombinationElement(copy, Menu, _popupMenu, combination);
        }
    }
}
