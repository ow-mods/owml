using UnityEngine.UI;
using OWML.ModHelper.Events;

namespace OWML.ModHelper.Menus
{
    public class ModToggleElement : ModInputElement<bool>
    {
        private readonly TwoButtonToggleElement _element;

        public ModToggleElement(TwoButtonToggleElement element): base(element)
        {
            _element = element;
            _element.GetValue<Button>("_buttonTrue").onClick.AddListener(() => InvokeOnChange(true));
            _element.GetValue<Button>("_buttonFalse").onClick.AddListener(() => InvokeOnChange(false));
        }

        public override bool Value
        {
            get => _element.GetValue();
            set => _element.Initialize(value);
        }
    }
}
