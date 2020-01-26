using OWML.Common.Menus;
using OWML.ModHelper.Events;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public abstract class ModInputField<T> : ModInput<T>
    {
        public IModButton Button { get; }

        protected readonly IModInputMenu InputMenu;
        protected readonly TwoButtonToggleElement ToggleElement;

        public ModInputField(TwoButtonToggleElement toggle, IModMenu menu, IModInputMenu inputMenu) : base(toggle, menu)
        {
            ToggleElement = toggle;
            InputMenu = inputMenu;
            Button = new ModButton(ToggleElement.GetValue<Button>("_buttonTrue"), menu);
            Button.OnClick += Open;

            var noButton = ToggleElement.GetValue<Button>("_buttonFalse");
            noButton.transform.parent.gameObject.SetActive(false);

            var layoutGroup = Button.Button.transform.parent.parent.GetComponent<HorizontalLayoutGroup>();
            layoutGroup.childControlWidth = true;
            layoutGroup.childForceExpandWidth = true;

            Button.Button.transform.parent.GetComponent<LayoutElement>().preferredWidth = 100;
        }

        protected abstract void Open();

    }
}
