using OWML.Common.Menus;
using UnityEngine;

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
            YesButton = menu.GetButton("Button-ON");
            YesButton.OnClick += () => InvokeOnChange(true);
            NoButton = menu.GetButton("Button-OFF");
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
