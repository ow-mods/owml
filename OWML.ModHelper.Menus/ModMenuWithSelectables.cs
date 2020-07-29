using System;
using System.Collections.Generic;
using System.Linq;
using OWML.Common.Menus;
using OWML.ModHelper.Events;
using OWML.ModHelper.Input;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModMenuWithSelectables : ModPopupMenu, IModMenuWithSelectables
    {
        public event Action OnCancel;

        protected List<Selectable> Selectables;
        protected ModCommandListener CommandListener;

        private void SetupCommands()
        {
            var listenerObject = new GameObject("ConfigurationMenu_Listener");
            CommandListener = listenerObject.AddComponent<ModCommandListener>();
            CommandListener.AddToListener(InputLibrary.confirm);
            CommandListener.AddToListener(InputLibrary.enter2); // keypad's Enter
            CommandListener.AddToListener(InputLibrary.cancel);
            CommandListener.AddToListener(InputLibrary.escape);
            CommandListener.AddToListener(InputLibrary.setDefaults);
            CommandListener.OnNewlyReleased += OnButton;
            listenerObject.SetActive(false);
        }

        protected virtual void SetupButtons()
        {
            var saveButton = GetPromptButton("UIElement-SaveAndExit");
            var resetButton = GetPromptButton("UIElement-ResetToDefaultsButton");
            var cancelButton = GetPromptButton("UIElement-DiscardChangesButton");

            if (saveButton == null || resetButton == null || cancelButton == null)
            {
                Console.WriteLine("Error - Failed to setup menu with selectables.");
                return;
            }

            saveButton.OnClick += OnSave;
            resetButton.OnClick += OnReset;
            cancelButton.OnClick += OnExit;

            saveButton.Prompt = new ScreenPrompt(InputLibrary.confirm, saveButton.DefaultTitle);
            cancelButton.Prompt = new ScreenPrompt(InputLibrary.cancel, cancelButton.DefaultTitle);
            resetButton.Prompt = new ScreenPrompt(InputLibrary.setDefaults, resetButton.DefaultTitle);
        }

        public override void Initialize(Menu menu)
        {
            var blocker = menu.GetComponentsInChildren<GraphicRaycaster>(true)
                .Single(x => x.name == "RebindingModeBlocker");
            blocker.gameObject.SetActive(false);

            var labelPanel = menu.GetValue<GameObject>("_selectableItemsRoot")
                .GetComponentInChildren<HorizontalLayoutGroup>(true);
            labelPanel.gameObject.SetActive(false);

            var layoutGroup = menu.GetComponentsInChildren<VerticalLayoutGroup>(true)
                .Single(x => x.name == "Content");
            Initialize(menu, layoutGroup);

            if (CommandListener == null)
            {
                SetupCommands();
            }
            SetupButtons();

            GetTitleButton("UIElement-CancelOutOfRebinding")?.Hide();
            GetTitleButton("UIElement-KeyRebinder")?.Hide();

            foreach (Transform child in layoutGroup.transform)
            {
                child.gameObject.SetActive(false);
            }
            Menu.OnActivateMenu += OnActivateMenu;
            Menu.OnDeactivateMenu += OnDeactivateMenu;
            UpdateNavigation();
        }

        public override void SelectFirst()
        {
            Locator.GetMenuInputModule().SelectOnNextUpdate(Selectables[0]);
            Menu.SetSelectOnActivate(Selectables[0]);
        }

        public override void UpdateNavigation()
        {
            Selectables = Layout.GetComponentsInChildren<TooltipSelectable>()
                .Select(x => x.GetComponent<Selectable>())
                .Where(x => x != null).ToList();
            UpdateNavigation(Selectables);
        }

        protected virtual void RemoveSelectable(Selectable selectable)
        {
            var index = Selectables.IndexOf(selectable);
            var upIndex = (index - 1 + Selectables.Count) % Selectables.Count;
            var downIndex = (index + 1) % Selectables.Count;
            var navigation = Selectables[upIndex].navigation;
            navigation.selectOnDown = Selectables[downIndex];
            Selectables[upIndex].navigation = navigation;
            navigation = Selectables[downIndex].navigation;
            navigation.selectOnUp = Selectables[upIndex];
            Selectables[downIndex].navigation = navigation;
            if (downIndex == 0)
            {
                Selectables[upIndex].Select();
            }
            else
            {
                Selectables[downIndex].Select();
            }

            Selectables.RemoveAt(index);
        }

        protected virtual void AddSelectable(Selectable selectable, int index)
        {
            Selectables.Insert(index, selectable);

            var current = Selectables[index];
            var next = Selectables[(index + 1) % Selectables.Count];
            var previous = Selectables[(index - 1 + Selectables.Count) % Selectables.Count];

            var navigation = next.navigation;
            navigation.selectOnUp = current;
            next.navigation = navigation;

            navigation = previous.navigation;
            navigation.selectOnDown = current;
            previous.navigation = navigation;

            navigation = current.navigation;
            navigation.selectOnDown = next;
            navigation.selectOnUp = previous;
            current.navigation = navigation;
        }

        protected virtual void AddSelectable(Selectable selectable)
        {
            AddSelectable(selectable, Selectables.Count);
        }

        protected virtual void OnActivateMenu()
        {
            CommandListener.gameObject.SetActive(true);
            CommandListener.BlockNextRelease();
        }

        protected virtual void OnDeactivateMenu()
        {
            CommandListener.gameObject.SetActive(false);
        }

        protected virtual void OnButton(SingleAxisCommand command)
        {
            command.ConsumeInput();
            if (command == InputLibrary.confirm && (OWInput.IsGamepadEnabled() || !InputLibrary.enter.GetValue<bool>("_blockNextRelease"))
                || command == InputLibrary.enter2)
            {
                OnSave();
            }
            if (command == InputLibrary.cancel || command == InputLibrary.escape)
            {
                OnExit();
            }
            if (command == InputLibrary.setDefaults)
            {
                OnReset();
            }
        }

        protected virtual void OnSave()
        {
            Close();
        }

        protected virtual void OnExit()
        {
            OnCancel?.Invoke();
            Close();
        }

        protected virtual void OnReset() { }
    }
}