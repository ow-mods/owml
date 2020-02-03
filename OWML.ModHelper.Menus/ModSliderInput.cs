using OWML.Common.Menus;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModSliderInput : ModInput<float>, IModSliderInput
    {
        public float Min { get; set; }
        public float Max { get; set; }

        private readonly SliderElement _element;

        public ModSliderInput(SliderElement element, IModMenu menu) : base(element, menu)
        {
            _element = element;
            element.OnValueChanged += () => InvokeOnChange(Value);
        }

        public override float Value
        {
            get => ToRealNumber(_element.GetValue());
            set => _element.Initialize((int)ToFakeNumber(value));
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

        private float ToRealNumber(float fakeNumber)
        {
            return fakeNumber * (Max - Min) / 10;
        }

        private float ToFakeNumber(float realNumber)
        {
            return realNumber * 10 / (Max - Min);
        }

    }
}
