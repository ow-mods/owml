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
        private SingleAxisCommand _cancelCommand;
        private string _comboName;
        private IModInputCombinationMenu _combinationMenu;
        private IModInputCombinationElement _element;

        public ModInputCombinationElementMenu(IModConsole console, IModInputHandler inputHandler) : base(console)
        {
            _inputHandler = inputHandler;
        }

        private GameObject CreateResetButton(Transform buttonsTransform)
        {
            var resetButtonObject = GameObject.Instantiate(buttonsTransform.GetChild(0).gameObject);
            resetButtonObject.name = "UIElement-ButtonReset";
            resetButtonObject.transform.SetParent(buttonsTransform);
            resetButtonObject.transform.SetSiblingIndex(1);
            resetButtonObject.transform.localScale = buttonsTransform.GetChild(0).localScale;
            return resetButtonObject;
        }

        private LayoutManager CreateLayoutManager(GameObject inputObject, GameObject resetButtonObject)
        {
            var layoutObject = inputObject.transform.GetChild(0).gameObject;
            var layoutGroupNew = layoutObject.AddComponent<HorizontalLayoutGroup>();
            layoutGroupNew.childForceExpandWidth = false;
            layoutGroupNew.childControlWidth = false;
            return new LayoutManager(layoutGroupNew, MonoBehaviour.FindObjectOfType<UIStyleManager>(),
                layoutObject.AddComponent<ModUIStyleApplier>(), resetButtonObject.transform.localScale);
        }

        public void Initialize(PopupInputMenu menu)
        {
            if (Menu != null)
            {
                return;
            }
            var parentCopy = GameObject.Instantiate(menu.transform.parent.gameObject);
            parentCopy.AddComponent<DontDestroyOnLoad>();
            _twoButtonPopup = parentCopy.transform.GetChild(0).GetComponent<PopupMenu>();
            var originalMenu = parentCopy.transform.GetComponentInChildren<PopupInputMenu>(true);//InputField-Popup
            var menuTransform = originalMenu.GetComponentInChildren<VerticalLayoutGroup>(true).transform;//InputFieldElements
            var buttonsTransform = menuTransform.GetComponentInChildren<HorizontalLayoutGroup>(true).transform;

            foreach (var button in buttonsTransform.GetComponentsInChildren<Button>())
            {
                button.navigation = new Navigation() { mode = Navigation.Mode.None };
            }
            Array.ForEach(menuTransform.GetComponentsInChildren<TabbedNavigation>(), navigation => GameObject.Destroy(navigation));


            var inputObject = menuTransform.GetComponentInChildren<InputField>(true).gameObject;//InputField
            GameObject.Destroy(inputObject.GetComponent<InputField>());
            for (int i = 3; i >= 1; i--)
            {
                GameObject.Destroy(inputObject.transform.GetChild(i).gameObject);
            }

            var resetButtonObject = CreateResetButton(buttonsTransform);
            var inputSelectable = inputObject.AddComponent<Selectable>();
            _inputMenu = originalMenu.gameObject.AddComponent<ModInputCombinationPopup>();
            _inputMenu.Initialize(originalMenu, inputSelectable, resetButtonObject.GetComponent<SubmitAction>(), 
                resetButtonObject.GetComponent<ButtonWithHotkeyImageElement>(), CreateLayoutManager(inputObject, resetButtonObject), _inputHandler);
            GameObject.Destroy(originalMenu);
            GameObject.Destroy(_inputMenu.GetValue<Text>("_labelText").GetComponent<LocalizedText>());
            Initialize((Menu)_inputMenu);
        }

        public void Open(string value, string comboName, IModInputCombinationMenu combinationMenu = null, IModInputCombinationElement element = null)
        {
            _combinationMenu = combinationMenu;
            _element = element;
            _comboName = comboName;
            _inputMenu.OnPopupConfirm += OnPopupConfirm;
            _inputMenu.OnPopupCancel += OnPopupCancel;
            _inputMenu.OnPopupValidate += OnPopupValidate;

            var message = "Press your combination";

            _inputMenu.EnableMenu(true, value);

            var okCommand = InputLibrary.confirm;
            var okPrompt = new ScreenPrompt(okCommand, "OK");
            if (_cancelCommand == null)
            {
                _cancelCommand = new SingleAxisCommand();
                var cancelBindingGmpd = new InputBinding(JoystickButton.Select);
                var cancelBindingKbrd = new InputBinding(KeyCode.Escape);
                _cancelCommand.SetInputs(cancelBindingGmpd, cancelBindingKbrd);
                var commandObject = new GameObject();
                var commandComponent = commandObject.AddComponent<ModCommandUpdater>();
                commandComponent.Initialize(_cancelCommand);
            }
            var cancelPrompt = new ScreenPrompt(_cancelCommand, "Cancel");
            var resetPrompt = new ScreenPrompt("Reset");
            _inputMenu.SetUpPopup(message, okCommand, _cancelCommand, null, okPrompt, cancelPrompt, resetPrompt);
            _inputMenu.GetValue<Text>("_labelText").text = message;
        }

        private bool OnPopupValidate()
        {
            var currentCombination = _inputMenu.Combination;
            _console.WriteLine($"Validating combination {currentCombination}");
            var collisions = _inputHandler.GetCollisions(currentCombination);
            if (collisions.Count > 0 && collisions[0] != _comboName)
            {
                _twoButtonPopup.EnableMenu(true);
                _twoButtonPopup.SetUpPopup($"this combination collides with \"{collisions[0]}\"", InputLibrary.confirm2, null,
                    new ScreenPrompt(InputLibrary.confirm2, "Ok"), new ScreenPrompt("Cancel"), true, false);
                _twoButtonPopup.GetValue<Text>("_labelText").text = $"this combination collides with \"{collisions[0]}\"";
                return false;
            }
            if (_combinationMenu == null)
            {
                return true;
            }
            foreach (var element in _combinationMenu.CombinationElements)
            {
                _console.WriteLine($"Checking against current {element.Title}");
                if (element.Title == currentCombination && element != _element)
                {
                    _twoButtonPopup.EnableMenu(true);
                    _twoButtonPopup.SetUpPopup($"This combination already exist in this group", InputLibrary.confirm2, null,
                        new ScreenPrompt(InputLibrary.confirm2, "Ok"), new ScreenPrompt("Cancel"), true, false);
                    _twoButtonPopup.GetValue<Text>("_labelText").text = $"This combination already exist in this group";
                    return false;
                }
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
