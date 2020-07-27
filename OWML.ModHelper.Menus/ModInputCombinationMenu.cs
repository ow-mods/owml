using System;
using System.Collections.Generic;
using System.Linq;
using OWML.Common;
using OWML.Common.Menus;
using Object = UnityEngine.Object;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModInputCombinationMenu : ModMenuWithSelectables, IModInputCombinationMenu
    {
        public event Action<string> OnConfirm;

        public List<IModInputCombinationElement> CombinationElements { get; }

        private IModInputCombinationElement _combinationElementTemplate;

        public ModInputCombinationMenu(IModConsole console, IModEvents events) : base(console, events)
        {
            CombinationElements = new List<IModInputCombinationElement>();
        }

        public string GenerateCombination()
        {
            var toDestroy = CombinationElements.Where(x => x.Title == "").ToList();
            toDestroy.ForEach(c => c.DestroySelf());
            return string.Join("/", CombinationElements.Select(x => x.Title).ToArray());
        }

        public void FillMenu(string combination)
        {
            Selectables = new List<Selectable>();
            CombinationElements.ForEach(element => element.Destroy());
            CombinationElements.Clear();
            combination.Split('/').ToList().ForEach(AddCombinationElement);
            SelectFirst();
            UpdateNavigation(Selectables);
        }

        protected override void SetupButtons()
        {
            base.SetupButtons();
            var addButton = GetPromptButton("UIElement-ResetToDefaultsButton");
            if (addButton == null)
            {
                Console.WriteLine("Error - Failed to setup combination menu");
                return;
            }
            addButton.Title = "Add Alternative";
        }

        public void Initialize(Menu menu, IModInputCombinationElement combinationElementTemplate)
        {
            if (Menu != null)
            {
                return;
            }
            _combinationElementTemplate = combinationElementTemplate;
            var canvasTransform = Object.Instantiate(menu.transform.parent.gameObject).transform;
            foreach (Transform child in canvasTransform)
            {
                Object.Destroy(child.gameObject);
            }
            menu.transform.SetParent(canvasTransform);
            var toggleTransform = _combinationElementTemplate.Toggle.transform;
            var oldScale = toggleTransform.localScale;
            toggleTransform.SetParent(canvasTransform);
            toggleTransform.localScale = oldScale;
            canvasTransform.gameObject.AddComponent<DontDestroyOnLoad>();

            base.Initialize(menu);
            Title = "Edit Combination";
        }

        public void RemoveCombinationElement(IModInputCombinationElement element)
        {
            CombinationElements.Remove(element);
            RemoveSelectable(element.Toggle.GetComponent<Selectable>());
        }

        private void AddCombinationElement(string combination)
        {
            AddCombinationElement(combination, CombinationElements.Count);
        }

        private void AddCombinationElement(string combination, int index)
        {
            var element = _combinationElementTemplate.Copy(combination);
            element.Show();
            var transform = element.Toggle.transform;
            var scale = transform.localScale;
            transform.parent = Layout.transform;
            element.Index = index;
            element.Initialize(this);
            CombinationElements.Add(element);
            element.Toggle.transform.localScale = scale;
            AddSelectable(element.Toggle.GetComponent<Selectable>(), index);
        }

        protected override void OnSave()
        {
            OnConfirm?.Invoke(GenerateCombination());
            base.OnSave();
        }

        protected override void OnReset()
        {
            AddCombinationElement("");
            Locator.GetMenuInputModule().SelectOnNextUpdate(Selectables[Selectables.Count - 1]);
        }
    }
}
