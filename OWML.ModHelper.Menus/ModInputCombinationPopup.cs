using OWML.Common;
using OWML.ModHelper.Events;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.ObjectModel;

namespace OWML.ModHelper.Menus
{
    public class ModInputCombinationPopup:PopupMenu
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
                        XboxPrefix + JoystickButtonToXboxButton(InputTranslator.ConvertKeyCodeToButton(_combination[j], OWInput.GetActivePadConfig())) :
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
        private List<KeyCode> _combination = new List<KeyCode>();
        protected SingleAxisCommand _resetCommand;

        private JoystickButton XboxButtonToJoystickButton(string xboxKey)
        {
            switch (xboxKey[0])
            {
                case 'A':
                    return JoystickButton.FaceDown;
                case 'B':
                    return JoystickButton.FaceRight;
                case 'X':
                    return JoystickButton.FaceLeft;
                case 'Y':
                    return JoystickButton.FaceUp;
                default:
                    return (JoystickButton)Enum.Parse(typeof(JoystickButton), xboxKey);
            }
        }

        private string JoystickButtonToXboxButton(JoystickButton key)
        {
            switch (key)
            {
                case JoystickButton.FaceDown:
                    return "A";
                case JoystickButton.FaceRight:
                    return "B";
                case JoystickButton.FaceLeft:
                    return "X";
                case JoystickButton.FaceUp:
                    return "Y";
                default:
                    return key.ToString();
            }
        }

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
            _combination.Clear();
            UpdateContents();
        }

        protected override void Update()
        {
            base.Update();
            if (_resetCommand != null && OWInput.IsNewlyPressed(_resetCommand, InputMode.All))
            {
                InvokeReset();
                OnPopupReset.Invoke();
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
                ButtonPromptLibrary.SharedInstance.GetButtonTexture(InputTranslator.ConvertKeyCodeToButton(key, OWInput.GetActivePadConfig())) :
                ButtonPromptLibrary.SharedInstance.GetButtonTexture(key)
                , Layout.ChildCount, ScaleDown);
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
            if (value)
            {
                _combination.Clear();
                foreach (var key in currentCombination.Split('+'))
                {
                    if (key != "")
                    {
                        _combination.Add(key.Contains(XboxPrefix) ?
                            InputTranslator.GetButtonKeyCode(XboxButtonToJoystickButton(key.Substring(XboxPrefix.Length))) :
                            (KeyCode)Enum.Parse(typeof(KeyCode), key));
                    }
                }
            }
            if (value && !_initialized)
            {
                InitializeMenu();
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
            _initialized = false;
            Layout = layout;
            this.InitializeMenu();
        }
    }
}
