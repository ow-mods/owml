using System;
using System.Collections.Generic;
using System.Linq;
using OWML.Common;
using OWML.Common.Menus;
using OWML.ModHelper.Events;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModInputCombinationMenu : ModPopupMenu, IModInputCombinationMenu
    {
        public event Action<string> OnConfirm;
        public event Action OnCancel;

        public List<IModInputCombinationElement> CombinationElements { get; private set; }

        private List<Selectable> _selectables;

        public string Combination {
            get
            {
                string result = "";
                for (int i = 0; i < CombinationElements.Count; i++)
                {
                    while (i < CombinationElements.Count && CombinationElements[i].Title == "")
                    {
                        CombinationElements[i].DestroySelf();
                    }
                }
                for (int i = 0; i < CombinationElements.Count; i++)
                {
                    if (CombinationElements[i].Title != "")
                    {
                        result += CombinationElements[i].Title;
                        if (i < CombinationElements.Count - 1)
                        {
                            result += "/";
                        }
                    }
                }
                return result;
            }
            set
            {
                _selectables = new List<Selectable>();
                CombinationElements.ForEach(element => element.Destroy());
                CombinationElements.Clear();
                foreach (var combination in value.Split('/'))
                {
                    AddCombinationElement(combination);
                }
                SelectFirst();
                UpdateNavigation(_selectables);
            }
        }

        private IModInputCombinationElement _combinationElementTemplate;

        public ModInputCombinationMenu(IModConsole console):base(console)
        {
            CombinationElements = new List<IModInputCombinationElement>();
        }

        public void Initialize(Menu menu, IModInputCombinationElement combinationElementTemplate)
        {
            _combinationElementTemplate = combinationElementTemplate;

            var blocker = menu.GetComponentsInChildren<GraphicRaycaster>().Single(x => x.name == "RebindingModeBlocker");
            blocker.gameObject.SetActive(false);

            var labelPanel = menu.GetValue<GameObject>("_selectableItemsRoot").GetComponentInChildren<HorizontalLayoutGroup>();
            labelPanel.gameObject.SetActive(false);

            var layoutGroup = menu.GetComponentsInChildren<VerticalLayoutGroup>().Single(x => x.name == "Content");
            Initialize(menu, layoutGroup);

            var saveButton = GetButton("UIElement-SaveAndExit");
            var addButton = GetButton("UIElement-ResetToDefaultsButton");
            var cancelButton = GetButton("UIElement-DiscardChangesButton");

            saveButton.OnClick += OnSave;
            addButton.OnClick += OnAdd;
            cancelButton.OnClick += OnExit;

            saveButton.SetControllerCommand(InputLibrary.confirm);
            cancelButton.SetControllerCommand(InputLibrary.cancel);
            addButton.SetControllerCommand(InputLibrary.setDefaults);

            var localText = addButton.Button.gameObject.GetComponentInChildren<LocalizedText>(true);
            if (localText != null)
            {
                GameObject.Destroy(localText);
            }
            var buttonWithHotkey = addButton.Button.gameObject.GetComponentInChildren<ButtonWithHotkeyImageElement>(true);
            if (buttonWithHotkey != null)
            {
                buttonWithHotkey.SetPrompt(new ScreenPrompt(InputLibrary.setDefaults,"Add Alternative"));
            }

            Title = "Edit Combination";

            GetButton("UIElement-CancelOutOfRebinding").Hide();
            GetButton("UIElement-KeyRebinder").Hide();

            for (int i = 0; i < layoutGroup.transform.childCount; i++)
            {
                layoutGroup.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        private void AddCombinationElement(string combination)
        {
            AddCombinationElement(combination, CombinationElements.Count);
        }

        private void AddCombinationElement(string combination, int index)
        {
            var element = _combinationElementTemplate.Copy(combination);
            var transform = element.Toggle.transform;
            var scale = transform.localScale;
            transform.parent = layoutGroup.transform;
            element.Index = index;
            element.Initialize(this);
            CombinationElements.Add(element);
            element.Toggle.transform.localScale = scale;
            _selectables.Add(element.Toggle.GetComponent<Selectable>());
        }

        private void OnSave()
        {
            OnConfirm?.Invoke(Combination);
            Close();
        }

        private void OnExit()
        {
            OnCancel?.Invoke();
            Close();
        }

        private void OnAdd()
        {
            AddCombinationElement("");
            var last = _selectables[_selectables.Count - 1];
            if (_selectables.Count > 1)
            {
                var first = _selectables[0];
                var prelast = _selectables[_selectables.Count - 2];

                var navigation = first.navigation;
                navigation.selectOnUp = last;
                first.navigation = navigation;

                navigation = prelast.navigation;
                navigation.selectOnDown = last;
                prelast.navigation = navigation;

                navigation = last.navigation;
                navigation.selectOnDown = first;
                navigation.selectOnUp = prelast;
                last.navigation = navigation;
            }
            Locator.GetMenuInputModule().SelectOnNextUpdate(last);
        }
    }
}
