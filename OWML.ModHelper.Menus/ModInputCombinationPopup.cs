using OWML.Common;
using OWML.Common.Menus;
using OWML.ModHelper.Input;
using OWML.ModHelper.Events;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace OWML.ModHelper.Menus
{
    public class ModInputCombinationPopup:PopupMenu
    {
        public ILayoutManager Layout { get; private set; }
        public event Action OnPopupReset;
        public string Combination
        {
            get
            {
                string result = "";
                for (var j = 0; j < _combination.Count; j++)
                {
                    result += ModInputLibrary.KeyCodeToString(_combination[j]);
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
            for (var code = ModInputLibrary.MinUsefulKey; code < ModInputLibrary.MaxUsefulKey; code++)
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
            Layout.AddPictureAt(ModInputLibrary.KeyTexture(key), Layout.ChildCount, ModInputLibrary.ScaleDown);
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
                        _combination.Add(ModInputLibrary.StringToKeyCode(key));
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

        public void Initialize(PopupMenu oldPopupMenu, Selectable defaultSelectable, SubmitAction resetAction, ButtonWithHotkeyImageElement resetButton, ILayoutManager layout)
        {
            var fields = typeof(PopupMenu).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            Array.ForEach<FieldInfo>(fields, field => field.SetValue(this, field.GetValue(oldPopupMenu)));
            _selectOnActivate = defaultSelectable;
            _resetAction = resetAction;
            _resetButton = resetButton;
            _initialized = false;
            Layout = layout;
            this.InitializeMenu();
        }
    }
}
