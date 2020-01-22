using OWML.Common.Menus;
using UnityEngine;

namespace OWML.ModHelper.Menus
{
    public class ModSliderInput : ModInput<float>, IModSliderInput
    {
        private readonly SliderElement _element;

        public ModSliderInput(SliderElement element, IModMenu menu): base(element, menu)
        {
            _element = element;
            element.OnValueChanged += () => InvokeOnChange(Value);
        }

        public override float Value
        {
            get => _element.GetValue();
            set => _element.Initialize((int)value);
        }

        public IModSliderInput Copy()
        {
            var copy = GameObject.Instantiate(_element);
            GameObject.Destroy(copy.GetComponentInChildren<LocalizedText>());
            return new ModSliderInput(copy, Menu);
        }

        public IModSliderInput Copy(string title)
        {
            var copy = Copy();
            copy.Title = title;
            return copy;
        }

    }
}
