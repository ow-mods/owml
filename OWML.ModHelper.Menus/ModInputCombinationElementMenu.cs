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
    public class ModInputCombinationElementMenu : ModTemporaryPopup, IModInputCombinationElementMenu
    {
        public event Action<string> OnConfirm;
        public IModMessagePopup MessagePopup { get; }

        private readonly IModInputHandler _inputHandler;
        private readonly IModPopupManager _popupManager;

        private ModInputCombinationPopup _inputMenu;
        private SingleAxisCommand _cancelCommand;
        private string _comboName;
        private IModInputCombinationMenu _combinationMenu;
        private IModInputCombinationElement _element;

        public ModInputCombinationElementMenu(IModInputHandler inputHandler, IModPopupManager popupManager)
        {
            MessagePopup = new ModMessagePopup();
            _inputHandler = inputHandler;
            _popupManager = popupManager;
        }

        private GameObject CreateResetButton(Transform buttonsTransform)
        {
            var template = buttonsTransform.GetComponentInChildren<ButtonWithHotkeyImageElement>(true).gameObject;
            var resetButtonObject = Object.Instantiate(template);
            resetButtonObject.transform.name = "UIElement-ButtonReset";
            resetButtonObject.name = "UIElement-ButtonReset";
            resetButtonObject.transform.SetParent(buttonsTransform);
            resetButtonObject.transform.SetSiblingIndex(1);
            resetButtonObject.transform.localScale = template.transform.localScale;
            return resetButtonObject;
        }

        private ModLayoutManager CreateLayoutManager(GameObject layoutObject, Transform scaleReference)
        {
            if (scaleReference == null)
            {
                ModConsole.OwmlConsole.WriteLine("Error - scale reference is null", MessageType.Error);
            }
            var layoutGroupNew = layoutObject.GetAddComponent<HorizontalLayoutGroup>();
            layoutGroupNew.childForceExpandWidth = false;
            layoutGroupNew.childControlWidth = false;
            var styleManager = Object.FindObjectOfType<UIStyleManager>();
            var styleApplier = layoutObject.AddComponent<ModUIStyleApplier>();
            return new ModLayoutManager(layoutGroupNew, styleManager, styleApplier, scaleReference.localScale);
        }

        private void Initialize(ModInputCombinationPopup menu)
        {
            Popup = menu;
            var menuTransform = menu.GetComponentInChildren<VerticalLayoutGroup>(true).transform; // InputFieldElements
            var fieldTransform = menuTransform.Find("InputField");
            var borderTransform = fieldTransform.Find("BorderImage");
            var layoutManager = CreateLayoutManager(borderTransform.gameObject,
                menuTransform.GetComponentInChildren<ButtonWithHotkeyImageElement>().transform);
            _inputMenu = menu;
            _inputMenu.Initialize(_inputHandler, layoutManager);
            base.Initialize(_inputMenu);
        }

        internal void Initialize(PopupInputMenu menu)
        {
            if (Menu != null)
            {
                return;
            }
            var menuTransform = menu.GetComponentInChildren<VerticalLayoutGroup>(true).transform; // InputFieldElements
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
            _inputMenu = menu.gameObject.AddComponent<ModInputCombinationPopup>();
            Popup = _inputMenu;
            var submitAction = resetButtonObject.GetComponent<SubmitAction>();
            var imageElement = resetButtonObject.GetComponent<ButtonWithHotkeyImageElement>();
            _inputMenu.Initialize(menu, inputSelectable, submitAction, imageElement, layout, _inputHandler);
            Object.Destroy(menu);
            Object.Destroy(_inputMenu.GetValue<Text>("_labelText").GetComponent<LocalizedText>());
            base.Initialize(_inputMenu);
        }

        internal void Open(string value, string comboName, IModInputCombinationMenu combinationMenu = null, IModInputCombinationElement element = null)
        {
            _combinationMenu = combinationMenu;
            _element = element;
            _comboName = comboName;
            RegisterEvents();

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

        internal override void DestroySelf()
        {
            DestroySelf(_inputMenu.gameObject);
            OnConfirm = null;
            _inputMenu = null;
        }

        internal ModInputCombinationElementMenu Copy()
        {
            var newPopupObject = CopyMenu();
            var newPopup = new ModInputCombinationElementMenu(_inputHandler, _popupManager);
            newPopup.Initialize(newPopupObject.GetComponent<ModInputCombinationPopup>());
            return newPopup;
        }

        private bool OnPopupValidate()
        {
            var currentCombination = _inputMenu.Combination;
            var collisions = _inputHandler.GetWarningMessages(currentCombination);
            collisions.Remove($"Collides with {_comboName}");
            if (collisions.Count > 0)
            {
                var problems = string.Join("\n", collisions.ToArray());
                var popup = _popupManager.CreateMessagePopup($"This combination has following problems:\n{problems}",
                    true, "Save anyway");
                popup.OnConfirm += OnForceConfirm;
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
            _popupManager.CreateMessagePopup("This combination already exist in this group");
            return false;
        }

        private void OnForceConfirm()
        {
            _inputMenu.EnableMenu(false);
            OnPopupConfirm();
        }

        protected override void OnPopupConfirm()
        {
            base.OnPopupConfirm();
            OnConfirm?.Invoke(_inputMenu.Combination);
        }

        protected override void RegisterEvents()
        {
            base.RegisterEvents();
            _inputMenu.OnPopupValidate += OnPopupValidate;
        }

        protected override void UnregisterEvents()
        {
            base.UnregisterEvents();
            _inputMenu.OnPopupValidate -= OnPopupValidate;
        }
    }
}
