using System;
using System.Collections.Generic;
using OWML.Common;
using OWML.Common.Menus;
using OWML.ModHelper.Events;
using UnityEngine.UI;
using UnityEngine;
using OWML.ModHelper.Input;

namespace OWML.ModHelper.Menus
{
    public class ModInputCombinationElementMenu : ModMenu, IModInputCombinationElementMenu
    {
        public event Action<string> OnConfirm;
        public event Action OnCancel;

        private ModInputCombinationPopup _inputMenu;
        private PopupMenu _twoButtonPopup;
        private IModInputHandler _inputHandler;

        public ModInputCombinationElementMenu(IModConsole console, IModInputHandler inputHandler) : base(console)
        {
            _inputHandler = inputHandler;
        }

        public void Initialize(PopupInputMenu menu)
        {//make more understandable
            if (Menu != null)
            {
                return;
            }
            var parent = menu.transform.parent.gameObject;
            var parentCopy = GameObject.Instantiate(parent);
            parentCopy.AddComponent<DontDestroyOnLoad>();
            _twoButtonPopup = parentCopy.transform.GetChild(0).GetComponent<PopupMenu>();
            var originalMenu = parentCopy.transform.GetComponentInChildren<PopupInputMenu>(true);//InputField-Popup
            var menuTransform = originalMenu.GetComponentInChildren<VerticalLayoutGroup>(true).transform;//InputFieldElements
            _inputMenu = originalMenu.gameObject.AddComponent<ModInputCombinationPopup>();
            var buttonsTransform = menuTransform.GetComponentInChildren<HorizontalLayoutGroup>(true).transform;
            foreach (var button in buttonsTransform.GetComponentsInChildren<Button>())
            {
                button.navigation = new Navigation() { mode = Navigation.Mode.None };
            }
            var resetButtonObject = GameObject.Instantiate(buttonsTransform.GetChild(0).gameObject);
            resetButtonObject.name = "UIElement-ButtonReset";
            resetButtonObject.transform.SetParent(buttonsTransform);
            resetButtonObject.transform.SetSiblingIndex(1);
            resetButtonObject.transform.localScale = buttonsTransform.GetChild(0).localScale;
            var resetAction = resetButtonObject.GetComponent<SubmitAction>();
            var resetButton = resetButtonObject.GetComponent<ButtonWithHotkeyImageElement>();
            var inputObject = menuTransform.GetComponentInChildren<InputField>(true).gameObject;
            GameObject.Destroy(inputObject.GetComponent<TabbedNavigation>());
            GameObject.Destroy(inputObject.GetComponent<InputField>());
            var inputSelectable = inputObject.AddComponent<Selectable>();
            var layoutObject = inputObject.transform.GetChild(0).gameObject;
            GameObject.Destroy(inputObject.transform.GetChild(3).GetComponent<Text>());
            var layoutGroupNew = layoutObject.AddComponent<HorizontalLayoutGroup>();
            layoutGroupNew.childControlWidth = false;
            layoutGroupNew.childForceExpandWidth = false;
            inputObject.transform.GetChild(1).GetComponent<Image>().color = Color.clear;
            var layout = new LayoutManager(layoutGroupNew, MonoBehaviour.FindObjectOfType<UIStyleManager>(),
                layoutObject.AddComponent<ModUIStyleApplier>(), resetButtonObject.transform.localScale);
            GameObject.Destroy(inputObject.transform.GetChild(2).gameObject);//destroy text
            GameObject.Destroy(menuTransform.GetChild(2).GetChild(0).GetComponent<TabbedNavigation>());
            GameObject.Destroy(menuTransform.GetChild(2).GetChild(1).GetComponent<TabbedNavigation>());
            GameObject.Destroy(menuTransform.GetChild(2).GetChild(2).GetComponent<TabbedNavigation>());
            _inputMenu.Initialize(originalMenu, inputSelectable, resetAction, resetButton, layout);
            GameObject.Destroy(originalMenu);
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
            var okPrompt = new ScreenPrompt("OK");
            var cancelCommand = new SingleAxisCommand();
            var cancelBinding = new InputBinding(JoystickButton.Select);
            cancelCommand.SetInputs(cancelBinding, null);
            var cancelPrompt = new ScreenPrompt("Cancel");
            var resetPrompt = new ScreenPrompt("Reset");
            _inputMenu.SetUpPopup(message, okCommand, cancelCommand, null, okPrompt, cancelPrompt, resetPrompt);
            _inputMenu.GetValue<Text>("_labelText").text = message;
        }

        private bool OnPopupValidate()
        {
            var collisions = _inputHandler.GetCollisions(_inputMenu.Combination); //probably should do it directly with string instead
            if (collisions.Count > 0)
            {
                _twoButtonPopup.EnableMenu(true);
                _twoButtonPopup.SetUpPopup($"this combination collides with \"{collisions[0]}\"", InputLibrary.confirm2, null,
                    new ScreenPrompt(InputLibrary.confirm2, "Ok"), new ScreenPrompt("Cancel"), true, false);
                _twoButtonPopup.GetValue<Text>("_labelText").text = $"this combination collides with \"{collisions[0]}\"";
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
