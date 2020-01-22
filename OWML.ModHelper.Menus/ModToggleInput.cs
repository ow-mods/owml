using OWML.Common.Menus;
using UnityEngine.UI;
using OWML.ModHelper.Events;
using UnityEngine;

namespace OWML.ModHelper.Menus
{
    public class ModToggleInput : ModInput<bool>, IModToggleInput
    {
        public IModButton YesButton { get; }
        public IModButton NoButton { get; }

        private readonly TwoButtonToggleElement _element;

        public ModToggleInput(TwoButtonToggleElement element, IModMenu menu): base(element, menu)
        {
            _element = element;
            YesButton = new ModButton(_element.GetValue<Button>("_buttonTrue"), menu);
            YesButton.OnClick += () => InvokeOnChange(true);
            NoButton = new ModButton(_element.GetValue<Button>("_buttonFalse"), menu);
            NoButton.OnClick += () => InvokeOnChange(false);
        }

        public override bool Value
        {
            get => _element.GetValue();
            set
            {
                _element.Initialize(value);
                InvokeOnChange(value);
            }
        }

        public IModToggleInput Copy()
        {
            var copy = GameObject.Instantiate(_element);
            GameObject.Destroy(copy.GetComponentInChildren<LocalizedText>());
            return new ModToggleInput(copy, Menu);
        }

        public IModToggleInput Copy(string title)
        {
            var copy = Copy();
            copy.Title = title;
            return copy;
        }

    }
}
