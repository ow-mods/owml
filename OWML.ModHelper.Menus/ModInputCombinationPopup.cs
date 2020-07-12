using OWML.Common;
using OWML.Common.Menus;
using OWML.ModHelper.Input;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;
using System.Linq;

namespace OWML.ModHelper.Menus
{
    public class ModInputCombinationPopup : PopupMenu
    {
        public IModLayoutManager Layout { get; private set; }
        public event Action OnPopupReset;
        public string Combination => string.Join("+", _combination.Select(ModInputLibrary.KeyCodeToString).ToArray());

        private SubmitAction _resetAction;
        private ButtonWithHotkeyImageElement _resetButton;
        private List<KeyCode> _combination = new List<KeyCode>();
        private IModInputHandler _inputHandler;
        private bool _wasReleased = true;
        private SingleAxisCommand _resetCommand;

        protected override void InitializeMenu()
        {
            base.InitializeMenu();
            if (_resetAction != null)
            {
                _resetAction.OnSubmitAction += InvokeReset;
            }
        }

        public override void Deactivate()
        {
            var component = _resetAction.GetComponent<UIStyleApplier>();
            component?.ChangeState(UIElementState.NORMAL, true);
            base.Deactivate();
        }

        protected virtual void InvokeReset()
        {
            _combination.Clear();
            UpdateContents();
            OnPopupReset?.Invoke();
        }

        private bool CheckCommands(KeyCode key)
        {
            if (_cancelCommand != null && _cancelCommand.IsNewlyPressed())
            {
                _cancelCommand.BlockNextRelease();
                InvokeCancel();
                return true;
            }
            if (_okCommand != null && _okCommand.IsNewlyPressed())
            {
                _okCommand.BlockNextRelease();
                InvokeOk();
                return true;
            }
            if (_resetCommand != null && _resetCommand.IsNewlyPressed())
            {
                _resetCommand.BlockNextRelease();
                InvokeReset();
                return true;
            }
            if (key == KeyCode.Mouse0 || key == KeyCode.Mouse1)
            {
                return true;
            }
            return false;
        }

        protected override void Update()
        {
            var currentlyPressedKeys = new List<KeyCode>();
            for (var code = ModInputLibrary.MinUsefulKey; code < ModInputLibrary.MaxUsefulKey; code++)
            {
                if (!(Enum.IsDefined(typeof(KeyCode), (KeyCode)code) && UnityEngine.Input.GetKey((KeyCode)code)))
                {
                    continue;
                }
                currentlyPressedKeys.Add((KeyCode)code);
            }
            if (currentlyPressedKeys.Count == 1 && CheckCommands(currentlyPressedKeys[0]))
            {
                return;
            }
            if (currentlyPressedKeys.Count < 8 && (currentlyPressedKeys.Count > _combination.Count
                || (currentlyPressedKeys.Count > 1 && _wasReleased)))
            {
                _wasReleased = false;
                _combination = currentlyPressedKeys;
                UpdateContents();
            }
            if (currentlyPressedKeys.Count == 0)
            {
                _wasReleased = true;
            }
        }

        public override void Activate()
        {
            base.Activate();
            Locator.GetMenuInputModule().SelectOnNextUpdate(null); // unselect buttons
        }

        private void AddKeySign(KeyCode key)
        {
            Layout.AddPictureAt(_inputHandler.Textures.KeyTexture(key), Layout.ChildCount, ModInputLibrary.ScaleDown);
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
                var keys = currentCombination.Split('+').Where(key => key != "").ToList();
                keys.ForEach(key => _combination.Add(ModInputLibrary.StringToKeyCode(key)));
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
            EnableMenu(value, "");
        }

        public void SetUpPopup(string message, SingleAxisCommand okCommand, SingleAxisCommand cancelCommand,
            SingleAxisCommand resetCommand, ScreenPrompt okPrompt, ScreenPrompt cancelPrompt,
            ScreenPrompt resetPrompt, bool closeMenuOnOk = true, bool setCancelButtonActive = true)
        {
            SetUpPopupCommandsShort(resetCommand, resetPrompt);
            base.SetUpPopup(message, okCommand, cancelCommand, okPrompt, cancelPrompt, closeMenuOnOk, setCancelButtonActive);
        }

        private void SetUpPopupCommandsShort(SingleAxisCommand resetCommand, ScreenPrompt resetPrompt)
        {
            _resetCommand = resetCommand;
            _resetButton.SetPrompt(resetPrompt);
        }

        public virtual void SetUpPopupCommands(SingleAxisCommand okCommand, SingleAxisCommand cancelCommand,
            SingleAxisCommand resetCommand, ScreenPrompt okPrompt, ScreenPrompt cancelPrompt, ScreenPrompt resetPrompt)
        {
            SetUpPopupCommandsShort(resetCommand, resetPrompt);
            base.SetUpPopupCommands(okCommand, cancelCommand, okPrompt, cancelPrompt);
        }

        public void Initialize(PopupMenu oldPopupMenu, Selectable defaultSelectable, SubmitAction resetAction,
            ButtonWithHotkeyImageElement resetButton, IModLayoutManager layout, IModInputHandler inputHandler)
        {
            _inputHandler = inputHandler;
            var fields = typeof(PopupMenu).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToList();
            fields.ForEach(field => field.SetValue(this, field.GetValue(oldPopupMenu)));
            _selectOnActivate = defaultSelectable;
            _resetAction = resetAction;
            _resetButton = resetButton;
            _initialized = false;
            Layout = layout;
            InitializeMenu();
        }
    }
}
