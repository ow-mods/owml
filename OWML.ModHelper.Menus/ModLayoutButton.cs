using OWML.Common.Interfaces.Menus;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModLayoutButton : ModButtonBase, IModLayoutButton
    {
        public IModLayoutManager Layout { get; }

        public ModLayoutButton(Button button, IModMenu menu) : base(button, menu)
        {
            var scale = button.transform.localScale;
            Object.Destroy(Button.GetComponentInChildren<Text>(true).gameObject);
            var layoutObject = new GameObject("LayoutGroup", typeof(RectTransform));
            layoutObject.transform.SetParent(button.transform);
            var target = layoutObject.AddComponent<Image>();
            target.raycastTarget = true;
            target.color = Color.clear;
            var layoutGroup = layoutObject.AddComponent<HorizontalLayoutGroup>();
            
            Initialize(menu);
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = false;
            var styleManager = Object.FindObjectOfType<UIStyleManager>();
            var styleApplier = ModUIStyleApplier.ReplaceStyleApplier(Button.gameObject);
            Layout = new ModLayoutManager(layoutGroup, styleManager, styleApplier, scale);
        }
    }
}
