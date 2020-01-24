using OWML.Common.Menus;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModSliderInput : ModInput<float>, IModSliderInput
    {
        private readonly SliderElement _element;
        private readonly Slider _slider;

        public ModSliderInput(SliderElement element, IModMenu menu) : base(element, menu)
        {
            _element = element;
            _slider = _element.GetComponentInChildren<Slider>();
            element.OnValueChanged += () => InvokeOnChange(Value);
        }

        public override float Value
        {
            get => _element.GetValue();
            set => _element.Initialize((int)value);
        }

        public float Min
        {
            get => _slider.minValue;
            set => _slider.minValue = value;
        }

        public float Max
        {
            get => _slider.maxValue;
            set => _slider.maxValue = value;
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
