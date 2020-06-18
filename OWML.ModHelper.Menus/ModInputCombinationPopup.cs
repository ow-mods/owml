using OWML.Common;
using OWML.ModHelper.Events;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.ObjectModel;

namespace OWML.ModHelper.Menus
{
    class ModInputCombinationPopup:PopupMenu
    {
        private const float ScaleDown = 0.75f;
        private const string XboxPrefix = "xbox_";
        private const int MinUsefulKey = 8;
        private const int MinGamepadKey = 330;
        private const int MaxUsefulKey = 350;

        public LayoutManager Layout { get; private set; }
        public event ModInputCombinationPopup.PopupResetEvent OnPopupReset;
        public delegate bool PopupResetEvent();
        public string Combination
        {
            get
            {
                string result = "";
                for (var j = 0; j < _combination.Count; j++)
                {
                    result += ((int)_combination[j]) >= MinGamepadKey ?
                        XboxPrefix + InputTranslator.GetXboxButtonFromJoysticKeyCode(_combination[j]).ToString() :
                        _combination[j].ToString();
                    if (j < _combination.Count - 1)
                    {
                        result += "+";
                    }
                }
                return result;
            }
        }
        public ReadOnlyCollection<KeyCode> KeyCodes => _combination.AsReadOnly();

        private SubmitAction _resetAction;
        private ButtonWithHotkeyImageElement _resetButton;
        private List<KeyCode> _combination;
        protected SingleAxisCommand _resetCommand;

        protected override void InitializeMenu()
        {
            base.InitializeMenu();
            if (this._resetAction != null)
            {
                this._resetAction.OnSubmitAction += this.InvokeReset;
            }
        }

        public override void Deactivate()
        {
            var component = this._resetAction.GetComponent<UIStyleApplier>();
            if (component != null)
            {
                component.ChangeState(UIElementState.NORMAL, true);
            }
            base.Deactivate();
        }

        protected virtual void InvokeReset()
        {
            _combination = new List<KeyCode>();
            UpdateContents();
        }

        protected override void Update()
        {
            base.Update();
            if (this._resetCommand != null && OWInput.IsNewlyPressed(this._resetCommand, InputMode.All))
            {
                this.InvokeReset();
            }
            List<KeyCode> currentlyPressedKeys = new List<KeyCode>();
            for (var code = MinUsefulKey; code < MaxUsefulKey; code++)
            {
                if (!(Enum.IsDefined(typeof(KeyCode), (KeyCode)code) && UnityEngine.Input.GetKey((KeyCode)code)))
                {
                    continue;
                }
                currentlyPressedKeys.Add((KeyCode)code);
            }
            if (currentlyPressedKeys.Count < 8 && currentlyPressedKeys.Count > _combination.Count)
            {
                _combination = currentlyPressedKeys;
                UpdateContents();
            }
        }

        public override void Activate()
        {
            base.Activate();
        }

        private void AddKeySign(KeyCode key)
        {
            Layout.AddPictureAt(
                ((int)key) >= MinGamepadKey ?
                InputTranslator.GetButtonTexture(InputTranslator.GetXboxButtonFromJoysticKeyCode(key)) :
                InputTranslator.GetButtonTexture(key)
                , Layout.ChildCount - 1, ScaleDown);
        }

        private void UpdateContents()
        {
            Layout.Clear();
            for (var j = 0; j < _combination.Count; j++)
            {
                AddKeySign(_combination[j]);
                if (j < _combination.Count - 1)
                {
                    Layout.AddText("+");
                }
            }
            Layout.UpdateState();
        }

        public void EnableMenu(bool value, string currentCombination)
        {
            _combination = new List<KeyCode>();
            foreach (var key in currentCombination.Split('+'))
            {
                _combination.Add(key.Contains(XboxPrefix) ?
                    InputTranslator.GetKeyCode((XboxButton)Enum.Parse(typeof(XboxButton), key.Substring(XboxPrefix.Length)), false) :
                    (KeyCode)Enum.Parse(typeof(KeyCode), key));
            }
            base.EnableMenu(value);
            UpdateContents();
        }

        public override void EnableMenu(bool value)
        {
            this.EnableMenu(value, "");
        }

        public void SetUpPopup(string message, SingleAxisCommand okCommand, SingleAxisCommand cancelCommand, SingleAxisCommand resetCommand, ScreenPrompt okPrompt, ScreenPrompt cancelPrompt, ScreenPrompt resetPrompt, bool closeMenuOnOk = true, bool setCancelButtonActive = true)
        {
            SetUpPopupCommandsShort(resetCommand, resetPrompt);
            base.SetUpPopup(message, okCommand, cancelCommand, okPrompt, cancelPrompt, closeMenuOnOk, setCancelButtonActive);
        }

        private void SetUpPopupCommandsShort(SingleAxisCommand resetCommand, ScreenPrompt resetPrompt)
        {
            this._resetCommand = resetCommand;
            this._resetButton.SetPrompt(resetPrompt, InputMode.Menu);
        }

        public virtual void SetUpPopupCommands(SingleAxisCommand okCommand, SingleAxisCommand cancelCommand, SingleAxisCommand resetCommand, ScreenPrompt okPrompt, ScreenPrompt cancelPrompt, ScreenPrompt resetPrompt)
        {
            SetUpPopupCommandsShort(resetCommand, resetPrompt);
            base.SetUpPopupCommands(okCommand, cancelCommand, okPrompt, cancelPrompt);
        }

        public void Initialize(PopupMenu oldPopupMenu, Selectable defaultSelectable, SubmitAction resetAction, ButtonWithHotkeyImageElement resetButton, LayoutManager layout)
        {
            _labelText = oldPopupMenu.GetValue<Text>("_labelText");
            _cancelAction = oldPopupMenu.GetValue<SubmitAction>("_cancelAction");
            _okAction = oldPopupMenu.GetValue<SubmitAction>("_okAction");
            _cancelButton = oldPopupMenu.GetValue<ButtonWithHotkeyImageElement>("_cancelButton");
            _confirmButton = oldPopupMenu.GetValue<ButtonWithHotkeyImageElement>("_confirmButton");
            _rootCanvas = oldPopupMenu.GetValue<Canvas>("_rootCanvas");
            _menuActivationRoot = oldPopupMenu.GetValue<GameObject>("_menuActivationRoot");
            _startEnabled = oldPopupMenu.GetValue<bool>("_startEnabled");
            _selectableItemsRoot = oldPopupMenu.GetValue<GameObject>("_selectableItemsRoot");
            _tooltipDisplay = oldPopupMenu.GetValue<TooltipDisplay>("_tooltipDisplay");
            _addToMenuStackManager = oldPopupMenu.GetValue<bool>("_addToMenuStackManager");
            _selectOnActivate = defaultSelectable;
            _resetAction = resetAction;
            _resetButton = resetButton;
            Layout = layout;
            base.InitializeMenu();
        }
    }
}
