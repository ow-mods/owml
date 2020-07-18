using OWML.Common.Menus;
using OWML.ModHelper.Events;
using UnityEngine;

namespace OWML.ModHelper.Menus
{
    public class ModSelectorInput : ModInput<int>, IModSelectorInput
    {
        public override bool IsSelected => _element?.GetValue<bool>("_amISelected") ?? false;

        private readonly OptionsSelectorElement _element;

        public ModSelectorInput(OptionsSelectorElement element, IModMenu menu) : base(element, menu)
        {
            _element = element;
            element.OnValueChanged += InvokeOnChange;
        }

        public void Initialize(int index, string[] options)
        {
            _element.Initialize(index, options);
        }

        public override int Value
        {
            get => _element.GetCurrentIndex();
            set => _element.Initialize(value);
        }

        public IModSelectorInput Copy()
        {
            var copy = Object.Instantiate(_element);
            Object.Destroy(copy.GetComponentInChildren<LocalizedText>(true));
            return new ModSelectorInput(copy, Menu);
        }

        public IModSelectorInput Copy(string title)
        {
            var copy = Copy();
            copy.Title = title;
            return copy;
        }
    }
}
