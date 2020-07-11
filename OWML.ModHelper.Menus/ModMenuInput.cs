using OWML.Common.Menus;
using OWML.ModHelper.Events;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public abstract class ModMenuInput<T> : ModInput<T>
    {
        protected readonly TwoButtonToggleElement ToggleElement;

        public override bool IsSelected => ToggleElement.GetValue<bool>("_amISelected");

        protected ModMenuInput(TwoButtonToggleElement toggle, IModMenu menu) : base(toggle, menu)
        {
            ToggleElement = toggle;

            var noButton = ToggleElement.GetValue<Button>("_buttonFalse");
            noButton.transform.parent.gameObject.SetActive(false);

            var buttonParent = toggle.GetValue<Button>("_buttonTrue").transform.parent;
            var layoutGroup = buttonParent.parent.GetComponent<HorizontalLayoutGroup>();
            layoutGroup.childControlWidth = true;
            layoutGroup.childForceExpandWidth = true;

            buttonParent.GetComponent<LayoutElement>().preferredWidth = 100;
        }

        protected void Subscribe(IModButton button)
        {
            button.OnClick += Open;
        }

        protected abstract void Open();

    }
}
