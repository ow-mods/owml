using OWML.Common.Menus;
using OWML.ModHelper.Events;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public class ModToggleInput : ModInput<bool>, IModToggleInput
    {
        public IModButton YesButton { get; }
        public IModButton NoButton { get; }

        public TwoButtonToggleElement Toggle { get; }

        public ModToggleInput(TwoButtonToggleElement toggle, IModMenu menu) : base(toggle, menu)
        {
            Toggle = toggle;
            YesButton = new ModButton(Toggle.GetValue<Button>("_buttonTrue"), menu);
            YesButton.OnClick += () => InvokeOnChange(true);
            NoButton = new ModButton(Toggle.GetValue<Button>("_buttonFalse"), menu);
            NoButton.OnClick += () => InvokeOnChange(false);
        }

        public override bool Value
        {
            get => Toggle.GetValue();
            set
            {
                Toggle.Initialize(value);
                InvokeOnChange(value);
            }
        }

        public IModToggleInput Copy()
        {
            var copy = GameObject.Instantiate(Toggle);
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
