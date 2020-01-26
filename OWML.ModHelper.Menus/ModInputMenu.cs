using System;
using OWML.Common;
using OWML.Common.Menus;
using UnityEngine;

namespace OWML.ModHelper.Menus
{
    public class ModInputMenu : ModMenu, IModInputMenu
    {
        public event Action<string> OnInput;

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
            Initialize((Menu)_inputMenu);
        }

        public void Open(InputType inputType, string value)
        {
            _inputMenu.OnPopupConfirm += OnConfirm;

            if (inputType == InputType.Number)
            {
                _inputMenu.OnInputPopupValidateChar += OnValidateCharNumber;
                _inputMenu.OnPopupValidate += OnValidateNumber;
            }

            var cancelCommand = OWInput.UsingGamepad() ? InputLibrary.cancel : InputLibrary.escape;
            var okPrompt = new ScreenPrompt(InputLibrary.confirm2, "OK", 0, false, false);
            var cancelPrompt = new ScreenPrompt(cancelCommand, "Cancel", 0, false, false);
            _inputMenu.SetUpPopup("Write the thing", InputLibrary.confirm2, cancelCommand, okPrompt, cancelPrompt, true, true);

            _inputMenu.SetInputFieldPlaceholderText(value);
            _inputMenu.EnableMenu(true);
        }

        private bool OnValidateNumber()
        {
            return float.TryParse(_inputMenu.GetInputText(), out _);
        }

        private bool OnValidateCharNumber(char c)
        {
            return "0123456789.".Contains("" + c);
        }

        private void OnConfirm()
        {
            _inputMenu.OnPopupValidate -= OnValidateNumber;
            _inputMenu.OnInputPopupValidateChar -= OnValidateCharNumber;
            _inputMenu.OnPopupConfirm -= OnConfirm;
            OnInput?.Invoke(_inputMenu.GetInputText());
        }

    }
}
