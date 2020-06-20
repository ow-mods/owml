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

        public List<IModInputCombinationElement> CombinationElements { get; private set; }

        public string Combination {
            get
            {
                string result = "";
                for (int i = 0; i < CombinationElements.Count; i++)
                {
                    result += CombinationElements[i].Title;
                    if (i<CombinationElements.Count-1)
                    {
                        result += "/";
                    }
                }
                return result;
            }
            set
            {
                CombinationElements.ForEach(x => x.DestroySelf());
                CombinationElements.Clear();
                foreach (var combination in value.Split('/'))
                {
                    AddCombinationElement(combination);
                    //CombinationElements.Add(_combinationElementTemplate.Copy(combination));
                }
                _console.WriteLine("Successfuly updated menu's combination");
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
            var resetButton = GetButton("UIElement-ResetToDefaultsButton");
            var cancelButton = GetButton("UIElement-DiscardChangesButton");

            saveButton.OnClick += OnSave;
            resetButton.OnClick += OnClear;
            cancelButton.OnClick += Close;

            saveButton.SetControllerCommand(InputLibrary.confirm);
            cancelButton.SetControllerCommand(InputLibrary.cancel);
            resetButton.SetControllerCommand(InputLibrary.setDefaults);

            resetButton.Title = "Clear";

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
        }

        private void OnSave()
        {
            OnConfirm?.Invoke(Combination);
            Close();
        }

        private void OnClear()
        {
            Combination = "";
        }
    }
}
