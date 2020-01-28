using System;
using OWML.Common;
using OWML.Common.Menus;
using OWML.ModHelper.Events;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModInputMenu : ModMenu, IModInputMenu
    {
        public event Action<string> OnConfirm;
        public event Action OnCancel;

        private PopupInputMenu _inputMenu;

        private IModLogger _logger;
        private IModConsole _console;

        public ModInputMenu(IModLogger logger, IModConsole console) : base(logger, console)
        {
            _logger = logger;
            _console = console;
        }

        public void Initialize(PopupInputMenu menu)
        {
            var parent = menu.transform.parent.gameObject;
            var parentCopy = GameObject.Instantiate(parent);
            parentCopy.AddComponent<DontDestroyOnLoad>();
            _inputMenu = parentCopy.transform.GetComponentInChildren<PopupInputMenu>(true);
            GameObject.Destroy(_inputMenu.GetValue<Text>("_labelText").GetComponent<LocalizedText>());
            Initialize((Menu)_inputMenu);
        }

        public void Open(InputType inputType, string value)
        {
            _inputMenu.OnPopupConfirm += OnPopupConfirm;
            _inputMenu.OnPopupCancel += OnPopupCancel;

            if (inputType == InputType.Number)
            {
                _inputMenu.OnInputPopupValidateChar += OnValidateCharNumber;
                _inputMenu.OnPopupValidate += OnValidateNumber;
            }
            var message = inputType == InputType.Number ? "Write a number" : "Write some text";

            _inputMenu.EnableMenu(true);

            var okPrompt = new ScreenPrompt(InputLibrary.confirm2, "OK");
            var cancelCommand = OWInput.UsingGamepad() ? InputLibrary.cancel : InputLibrary.escape;
            var cancelPrompt = new ScreenPrompt(cancelCommand, "Cancel");
            _inputMenu.SetUpPopup(message, InputLibrary.confirm2, cancelCommand, okPrompt, cancelPrompt, true, true);
            _inputMenu.SetInputFieldPlaceholderText("");
            _inputMenu.GetInputField().text = value;
            _inputMenu.GetValue<Text>("_labelText").text = message;
        }

        private bool OnValidateNumber()
        {
            return float.TryParse(_inputMenu.GetInputText(), out _);
        }

        private bool OnValidateCharNumber(char c)
        {
            return "0123456789.".Contains("" + c);
        }

        private void OnPopupConfirm()
        {
            UnregisterEvents();
            OnConfirm?.Invoke(_inputMenu.GetInputText());
        }

        private void OnPopupCancel()
        {
            UnregisterEvents();
            OnCancel?.Invoke();
        }

        private void UnregisterEvents()
        {
            _inputMenu.OnPopupValidate -= OnValidateNumber;
            _inputMenu.OnInputPopupValidateChar -= OnValidateCharNumber;
            _inputMenu.OnPopupConfirm -= OnPopupConfirm;
            _inputMenu.OnPopupCancel -= OnPopupCancel;
        }

    }
}
