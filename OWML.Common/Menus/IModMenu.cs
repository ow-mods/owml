using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace OWML.Common.Menus
{
    public interface IModMenu
    {
        event Action OnInit;

        Menu Menu { get; }

        List<IBaseButton> Buttons { get; }
        List<IModTitleButton> TitleButtons { get; }
        List<IModLayoutButton> LayoutButtons { get; }

        IModTitleButton GetButton(string title);
        IBaseButton AddButton(IBaseButton button);
        IBaseButton AddButton(IBaseButton button, int index);

        List<IModToggleInput> ToggleInputs { get; }
        IModToggleInput GetToggleInput(string title);
        IModToggleInput AddToggleInput(IModToggleInput input);
        IModToggleInput AddToggleInput(IModToggleInput input, int index);

        List<IModSliderInput> SliderInputs { get; }
        IModSliderInput GetSliderInput(string title);
        IModSliderInput AddSliderInput(IModSliderInput input);
        IModSliderInput AddSliderInput(IModSliderInput input, int index);

        List<IModTextInput> TextInputs { get; }
        IModTextInput GetTextInput(string title);
        IModTextInput AddTextInput(IModTextInput input);
        IModTextInput AddTextInput(IModTextInput input, int index);

        List<IModNumberInput> NumberInputs { get; }
        IModNumberInput GetNumberInput(string title);
        IModNumberInput AddNumberInput(IModNumberInput input);
        IModNumberInput AddNumberInput(IModNumberInput input, int index);

        object GetInputValue(string key);
        void SetInputValue(string key, object value);

        void SelectFirst();
        void UpdateNavigation();

        [Obsolete("Use Buttons instead")]
        List<Button> GetButtons();
        [Obsolete("Use button.Duplicate instead")]
        Button AddButton(string title, int index);
    }
}
