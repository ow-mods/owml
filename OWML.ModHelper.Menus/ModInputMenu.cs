using System;
using OWML.Common;
using OWML.Common.Menus;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModInputMenu : ModMenu, IModInputMenu
    {
        public event Action<string> OnInput;

        private PopupInputMenu _inputMenu;
        private InputField _inputField;

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
            _inputMenu = parentCopy.transform.GetChild(1).GetComponent<PopupInputMenu>();
            _inputField = _inputMenu.GetComponentInChildren<InputField>();
            Initialize((Menu)_inputMenu);
        }

        public void Open(InputField.ContentType contentType, InputField.CharacterValidation validation, string value)
        {
            _inputMenu.OnPopupConfirm += () => OnInput?.Invoke(_inputMenu.GetInputText());

            var cancelCommand = OWInput.UsingGamepad() ? InputLibrary.cancel : InputLibrary.escape;
            _inputMenu.SetUpPopup("Write the thing", InputLibrary.confirm2, cancelCommand, /*this._confirmCreateProfilePrompt*/null, /*cancelPrompt*/null, true, false);

            _inputField.characterValidation = validation;
            _inputField.contentType = contentType;

            _inputMenu.SetInputFieldPlaceholderText(value);
            _inputMenu.EnableMenu(true);
        }

    }
}
