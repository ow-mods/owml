using OWML.Common.Menus;
using UnityEngine;

namespace OWML.ModHelper.Menus
{
    public class ModSliderElement : ModInputElement<float>
    {
        private readonly SliderElement _element;

        public ModSliderElement(SliderElement element): base(element)
        {
            _element = element;
            element.OnValueChanged += () => InvokeOnChange(Value);
        }

        public override float Value
        {
            get => _element.GetValue();
            set => _element.Initialize((int)value);
        }

        public override IModInput<float> Copy()
        {
            var copy = GameObject.Instantiate(_element);
            return new ModSliderElement(copy);
        }
    }
}
