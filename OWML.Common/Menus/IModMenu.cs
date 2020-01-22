using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace OWML.Common.Menus
{
    public interface IModMenu
    {
        event Action OnInit;

        Menu Menu { get; }

        List<IModButton> Buttons { get; }
        IModButton GetButton(string title);
        IModButton AddButton(IModButton button);
        IModButton AddButton(IModButton button, int index);

        List<IModToggleInput> ToggleInputs { get; }
        IModToggleInput GetToggleInput(string title);
        IModToggleInput AddToggleInput(IModToggleInput input);
        IModToggleInput AddToggleInput(IModToggleInput input, int index);

        List<IModSliderInput> SliderInputs { get; }
        IModSliderInput GetSliderInput(string title);
        IModSliderInput AddSliderInput(IModSliderInput input);
        IModSliderInput AddSliderInput(IModSliderInput input, int index);

        [Obsolete("Use Buttons instead")]
        List<Button> GetButtons();
        [Obsolete("Use button.Duplicate instead")]
        Button AddButton(string title, int index);
    }
}
