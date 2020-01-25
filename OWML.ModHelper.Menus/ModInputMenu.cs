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
            _inputMenu = parentCopy.transform.GetChild(1).GetComponent<PopupInputMenu>();
            Initialize((Menu)_inputMenu);
        }

        public void Open()
        {
            Open("...");
        }

        public void Open(string placeholderText)
        {
            _inputMenu.OnPopupConfirm += () => OnInput?.Invoke(_inputMenu.GetInputText());

            //_inputMenu.OnPopupValidate += OnCreateProfileValidate;
            //_inputMenu.OnInputPopupValidateChar += OnValidateChar;

            var cancelCommand = OWInput.UsingGamepad() ? InputLibrary.cancel : InputLibrary.escape;
            _inputMenu.SetUpPopup("Write the thing", InputLibrary.confirm2, cancelCommand, /*this._confirmCreateProfilePrompt*/null, /*cancelPrompt*/null, true, false);

            _inputMenu.SetInputFieldPlaceholderText(placeholderText);
            _inputMenu.EnableMenu(true);
        }

    }
}
