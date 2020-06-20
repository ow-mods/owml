using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OWML.Common;
using OWML.ModHelper.Events;
using UnityEngine.UI;
using UnityEngine;
using System.CodeDom;
using OWML.ModHelper.Input;
using System.Collections.ObjectModel;

namespace OWML.ModHelper.Menus
{
    public class ModInputCombinationElementMenu : ModMenu
    {
        private const int MaxUsefulKey = 350;

        public event Action<string> OnConfirm;
        public event Action OnCancel;

        private ModInputCombinationPopup _inputMenu;
        private PopupMenu _popup;
        private IModInputHandler _inputHandler;

        public ModInputCombinationElementMenu(IModConsole console, IModInputHandler inputHandler) : base(console)
        {
            _inputHandler = inputHandler;
        }

        public void Initialize(PopupInputMenu menu)
        {
            if (Menu != null)
            {
                return;
            }
            var parent = menu.transform.parent.gameObject;
            var parentCopy = GameObject.Instantiate(parent);
            parentCopy.AddComponent<DontDestroyOnLoad>();
            _popup = parentCopy.transform.GetChild(0).GetComponent<PopupMenu>();
            var originalMenu = parentCopy.transform.GetComponentInChildren<PopupInputMenu>(true);
            var menuTransform = originalMenu.transform.GetChild(1).GetChild(2);
            _inputMenu = originalMenu.gameObject.AddComponent<ModInputCombinationPopup>();
            var resetButtonObject = GameObject.Instantiate(menuTransform.GetChild(2).GetChild(0).gameObject);
            resetButtonObject.name = "UIElement-ButtonReset";
            resetButtonObject.transform.SetParent(menuTransform.GetChild(2));
            resetButtonObject.transform.SetSiblingIndex(1);
            var resetAction = resetButtonObject.GetComponent<SubmitAction>();
            var resetButton = resetButtonObject.GetComponent<ButtonWithHotkeyImageElement>();
            var inputObject = menuTransform.GetChild(1).gameObject;
            GameObject.Destroy(inputObject.GetComponent<TabbedNavigation>());
            GameObject.Destroy(inputObject.GetComponent<InputField>());
            var inputSelectable = inputObject.AddComponent<Selectable>();
            var layoutObject = inputObject.transform.GetChild(3).gameObject;
            GameObject.Destroy(layoutObject.GetComponent<Text>());
            layoutObject.name = "Combination";
            var layoutGroupNew = layoutObject.AddComponent<HorizontalLayoutGroup>();
            var layout = new LayoutManager(layoutGroupNew, MonoBehaviour.FindObjectOfType<UIStyleManager>(),
                layoutObject.AddComponent<UIStyleApplier>(), layoutObject.transform.localScale);
            GameObject.Destroy(inputObject.transform.GetChild(2).gameObject);//destroy text
            GameObject.Destroy(menuTransform.GetChild(2).GetChild(0).GetComponent<TabbedNavigation>());
            GameObject.Destroy(menuTransform.GetChild(2).GetChild(1).GetComponent<TabbedNavigation>());
            GameObject.Destroy(menuTransform.GetChild(2).GetChild(2).GetComponent<TabbedNavigation>());
            _inputMenu.Initialize(originalMenu, inputSelectable, resetAction, resetButton, layout);
            GameObject.Destroy(_inputMenu.GetValue<Text>("_labelText").GetComponent<LocalizedText>());
            Initialize((Menu)_inputMenu);
        }

        public void Open(string value)
        {
            _inputMenu.OnPopupConfirm += OnPopupConfirm;
            _inputMenu.OnPopupCancel += OnPopupCancel;
            _inputMenu.OnPopupValidate += OnPopupValidate;

            var message = "Press your combination";

            _inputMenu.EnableMenu(true, value);

            var okCommand = new SingleAxisCommand();
            var okBinding = new InputBinding(JoystickButton.Start);
            okCommand.SetInputs(okBinding, null);
            var okPrompt = new ScreenPrompt(InputLibrary.confirm2, "OK");
            var cancelCommand = new SingleAxisCommand();
            var cancelBinding = new InputBinding(JoystickButton.Select);
            cancelCommand.SetInputs(cancelBinding, null);
            var cancelPrompt = new ScreenPrompt(cancelCommand, "Cancel");
            var resetPrompt = new ScreenPrompt("Reset");
            _inputMenu.SetUpPopup(message, okCommand, cancelCommand, null, okPrompt, cancelPrompt, resetPrompt);
            _inputMenu.GetValue<Text>("_labelText").text = message;
        }

        private bool OnPopupValidate()
        {
            var Codes = _inputMenu.KeyCodes;
            long hash = 0;
            foreach (var code in Codes)
            {
                hash = hash * MaxUsefulKey + (long)code;
            }
            var hashes = new List<long>();
            hashes.Add(hash);
            var collisions = _inputHandler.GetCollisions(hashes.AsReadOnly()); //probably should do it directly with string instead
            if (collisions.Count > 0)
            {
                _popup.EnableMenu(true);
                _popup.SetUpPopup($"this combination collides with \"{collisions[0]}\"", InputLibrary.confirm2, null, 
                    new ScreenPrompt(InputLibrary.confirm2, "Ok"), new ScreenPrompt("Cancel"), true, false);
                _popup.GetValue<Text>("_labelText").text = $"this combination collides with \"{collisions[0]}\"";
                return false;
            }
            return true;
        }

        private void OnPopupConfirm()
        {
            UnregisterEvents();
            OnConfirm?.Invoke(_inputMenu.Combination);
        }

        private void OnPopupCancel()
        {
            UnregisterEvents();
            OnCancel?.Invoke();
        }

        private void UnregisterEvents()
        {
            _inputMenu.OnPopupConfirm -= OnPopupConfirm;
            _inputMenu.OnPopupCancel -= OnPopupCancel;
        }
    }
}
