using OWML.Common.Menus;
using OWML.ModHelper.Events;
using UnityEngine;

namespace OWML.ModHelper.Menus
{
    public class ModSliderInput : ModInput<float>, IModSliderInput
    {
        public float Min { get; set; }
        public float Max { get; set; }
        public override bool IsSelected => _uIStyleApplier?.GetValue<bool>("_selected") ?? false;

        private readonly SliderElement _element;
        private readonly UIStyleApplier _uIStyleApplier;

        public ModSliderInput(SliderElement element, IModMenu menu) : base(element, menu)
        {
            _element = element;
            _uIStyleApplier = element.GetComponent<UIStyleApplier>();
            element.OnValueChanged += () => InvokeOnChange(Value);
        }

        public override float Value
        {
            get => ToRealNumber(_element.GetValue());
            set => _element.Initialize((int)ToFakeNumber(value));
        }

        public IModSliderInput Copy()
        {
            var copy = Object.Instantiate(_element);
            Object.Destroy(copy.GetComponentInChildren<LocalizedText>(true));
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
