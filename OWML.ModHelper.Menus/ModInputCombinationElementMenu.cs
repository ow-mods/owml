using System;
using OWML.Common;
using OWML.Common.Menus;
using OWML.ModHelper.Events;
using UnityEngine.UI;
using UnityEngine;
using OWML.ModHelper.Input;
using System.Linq;
using OWML.Logging;
using Object = UnityEngine.Object;

namespace OWML.ModHelper.Menus
{
    public class ModInputCombinationElementMenu : ModMenu, IModInputCombinationElementMenu
    {
        public event Action<string> OnConfirm;
        public event Action OnCancel;

        private readonly IModInputHandler _inputHandler;

        private ModInputCombinationPopup _inputMenu;
        private PopupMenu _twoButtonPopup;
        private SingleAxisCommand _cancelCommand;
        private string _comboName;
        private IModInputCombinationMenu _combinationMenu;
        private IModInputCombinationElement _element;

        public ModInputCombinationElementMenu(IModInputHandler inputHandler)
        {
            _inputHandler = inputHandler;
        }

        private GameObject CreateResetButton(Transform buttonsTransform)
        {
            var template = buttonsTransform.GetComponentInChildren<ButtonWithHotkeyImageElement>(true).gameObject;
            var resetButtonObject = Object.Instantiate(template);
            resetButtonObject.name = "UIElement-ButtonReset";
            resetButtonObject.transform.SetParent(buttonsTransform);
            resetButtonObject.transform.SetSiblingIndex(1);
            resetButtonObject.transform.localScale = template.transform.localScale;
            return resetButtonObject;
        }

        private ModLayoutManager CreateLayoutManager(GameObject layoutObject, Transform scaleReference)
        {
            var layoutGroupNew = layoutObject.AddComponent<HorizontalLayoutGroup>();
            layoutGroupNew.childForceExpandWidth = false;
            layoutGroupNew.childControlWidth = false;
            var styleManager = Object.FindObjectOfType<UIStyleManager>();
            var styleApplier = layoutObject.AddComponent<ModUIStyleApplier>();
            return new ModLayoutManager(layoutGroupNew, styleManager, styleApplier, scaleReference.localScale);
        }

        public void Initialize(PopupInputMenu menu)
        {
            if (Menu != null)
            {
                return;
            }
            var parentCopy = Object.Instantiate(menu.transform.parent.gameObject);
            parentCopy.AddComponent<DontDestroyOnLoad>();
            _twoButtonPopup = parentCopy.transform.Find("TwoButton-Popup")?.GetComponent<PopupMenu>();
            if (_twoButtonPopup == null)
            {
                Console.WriteLine("Error - Failed to setup warning popup.");
            }

            var originalMenu = parentCopy.transform.GetComponentInChildren<PopupInputMenu>(true); // InputField-Popup
            var menuTransform = originalMenu.GetComponentInChildren<VerticalLayoutGroup>(true).transform; // InputFieldElements
            var buttonsTransform = menuTransform.GetComponentInChildren<HorizontalLayoutGroup>(true).transform;

            var buttons = buttonsTransform.GetComponentsInChildren<Button>(true).ToList();
            buttons.ForEach(button => button.navigation = new Navigation { mode = Navigation.Mode.None });
            var tabbedNavigations = menuTransform.GetComponentsInChildren<TabbedNavigation>(true).ToList();
            tabbedNavigations.ForEach(Object.Destroy);

            var resetButtonObject = CreateResetButton(buttonsTransform);
            ModLayoutManager layout = null;

            var inputObject = menuTransform.GetComponentInChildren<InputField>(true).gameObject; // InputField
            Object.Destroy(inputObject.GetComponent<InputField>());
            foreach (Transform child in inputObject.transform)
            {
                if (child.name == "BorderImage")
                {
                    layout = CreateLayoutManager(child.gameObject, resetButtonObject.transform);
                }
                else
                {
                    Object.Destroy(child.gameObject);
                }
            }

            if (layout == null)
            {
                ModConsole.OwmlConsole.WriteLine("Error - Failed to create combination visualizer in combination editor.", MessageType.Error);
                return;
            }

            var inputSelectable = inputObject.AddComponent<Selectable>();
            _inputMenu = originalMenu.gameObject.AddComponent<ModInputCombinationPopup>();
            var submitAction = resetButtonObject.GetComponent<SubmitAction>();
            var imageElement = resetButtonObject.GetComponent<ButtonWithHotkeyImageElement>();
            _inputMenu.Initialize(originalMenu, inputSelectable, submitAction, imageElement, layout, _inputHandler);
            Object.Destroy(originalMenu);
            Object.Destroy(_inputMenu.GetValue<Text>("_labelText").GetComponent<LocalizedText>());
            Initialize(_inputMenu);
        }

        public void Open(string value, string comboName, IModInputCombinationMenu combinationMenu = null, IModInputCombinationElement element = null)
        {
            _combinationMenu = combinationMenu;
            _element = element;
            _comboName = comboName;
            _inputMenu.OnPopupConfirm += OnPopupConfirm;
            _inputMenu.OnPopupCancel += OnPopupCancel;
            _inputMenu.OnPopupValidate += OnPopupValidate;

            const string message = "Press your combination";

            _inputMenu.EnableMenu(true, value);

            var okCommand = InputLibrary.confirm;
            var okPrompt = new ScreenPrompt(okCommand, "OK");
            if (_cancelCommand == null)
            {
                _cancelCommand = new SingleAxisCommand();
                var cancelBindingGamepad = new InputBinding(JoystickButton.Select);
                var cancelBindingKeyboard = new InputBinding(KeyCode.Escape);
                _cancelCommand.SetInputs(cancelBindingGamepad, cancelBindingKeyboard);
                var commandObject = new GameObject();
                var commandComponent = commandObject.AddComponent<ModCommandUpdater>();
                commandComponent.Initialize(_cancelCommand);
            }
            var cancelPrompt = new ScreenPrompt(_cancelCommand, "Cancel");
            var resetPrompt = new ScreenPrompt("Reset");
            _inputMenu.SetUpPopup(message, okCommand, _cancelCommand, null, okPrompt, cancelPrompt, resetPrompt);
            _inputMenu.GetValue<Text>("_labelText").text = message;
        }

        private void ShowWarningPopup(string message, bool addCancel = false, string okMessage = "OK")
        {
            if (_twoButtonPopup == null)
            {
                Console.WriteLine("Failed to create popup for a following message:");
                Console.WriteLine(message);
            }
            _twoButtonPopup.EnableMenu(true);
            _twoButtonPopup.SetUpPopup(message, InputLibrary.confirm, addCancel ? InputLibrary.cancel : null,
                new ScreenPrompt(InputLibrary.confirm, okMessage), new ScreenPrompt("Cancel"), true, addCancel);
            _twoButtonPopup.GetValue<Text>("_labelText").text = message;
        }

        private bool OnPopupValidate()
        {
            var currentCombination = _inputMenu.Combination;
            var collisions = _inputHandler.GetWarningMessages(currentCombination);
            collisions.Remove($"Collides with {_comboName}");
            if (collisions.Count > 0)
            {
                ShowWarningPopup($"This combination has following problems:\n{string.Join("\n", collisions.ToArray())}",
                    true, "Save anyway");
                _twoButtonPopup.OnPopupConfirm += OnForceConfirm;
                _twoButtonPopup.OnPopupCancel += DisableWarningSubscription;
                return false;
            }
            if (_combinationMenu == null)
            {
                return true;
            }
            var overlap = _combinationMenu.CombinationElements
                .Any(element => element.Title == currentCombination && element != _element);
            if (!overlap)
            {
                return true;
            }
            ShowWarningPopup("This combination already exist in this group");
            return false;
        }

        private void DisableWarningSubscription()
        {
            _twoButtonPopup.OnPopupConfirm -= OnForceConfirm;
            _twoButtonPopup.OnPopupCancel -= DisableWarningSubscription;
        }

        private void OnForceConfirm()
        {
            DisableWarningSubscription();
            _inputMenu.EnableMenu(false);
            OnPopupConfirm();
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
