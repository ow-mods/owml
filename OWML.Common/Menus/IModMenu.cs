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

        List<IModInput<bool>> ToggleInputs { get; }
        IModInput<bool> AddToggleInput(IModInput<bool> input);
        IModInput<bool> AddToggleInput(IModInput<bool> input, int index);

        List<IModInput<float>> SliderInputs { get; }
        IModInput<float> AddSliderInput(IModInput<float> input);
        IModInput<float> AddSliderInput(IModInput<float> input, int index);

        [Obsolete("Use Buttons instead")]
        List<Button> GetButtons();
        [Obsolete("Use button.Duplicate instead")]
        Button AddButton(string title, int index);
    }
}
