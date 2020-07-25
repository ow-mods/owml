using System;
using OWML.Common;
using OWML.Common.Menus;
using OWML.ModHelper.Events;
using Object = UnityEngine.Object;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModInputMenu : ModTemporaryPopup, IModInputMenu
    {
        public event Action<string> OnConfirm;
        public event Action OnCancel;

        private PopupInputMenu _inputMenu;

        public ModInputMenu(IModConsole console) : base(console) { }

        internal void Initialize(PopupInputMenu menu)
        {
            if (Menu != null)
            {
                return;
            }
            var parent = menu.transform.parent.gameObject;
            var parentCopy = Object.Instantiate(parent);
            parentCopy.AddComponent<DontDestroyOnLoad>();
            _inputMenu = parentCopy.transform.GetComponentInChildren<PopupInputMenu>(true);
            Object.Destroy(_inputMenu.GetValue<Text>("_labelText").GetComponent<LocalizedText>());
            Initialize((Menu)_inputMenu);
        }

        internal void Open(InputType inputType, string value)
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
            _inputMenu.SetUpPopup(message, InputLibrary.confirm2, cancelCommand, okPrompt, cancelPrompt);
            _inputMenu.SetInputFieldPlaceholderText("");
            _inputMenu.GetInputField().text = value;
            _inputMenu.GetValue<Text>("_labelText").text = message;
        }

        internal ModInputMenu Copy()
        {
            var newPopupObject = Object.Instantiate(_inputMenu.gameObject);
            newPopupObject.transform.SetParent(_inputMenu.transform.parent);
            newPopupObject.transform.localScale = _inputMenu.transform.localScale;
            newPopupObject.transform.localPosition = _inputMenu.transform.localPosition;
            var newPopup = new ModInputMenu(OwmlConsole);
            newPopup.Initialize(newPopupObject.GetComponent<PopupInputMenu>());
            return newPopup;
        }

        internal override void DestroySelf()
        {
            Object.Destroy(_inputMenu);
            OnConfirm = null;
            OnCancel = null;
            _inputMenu = null;
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
