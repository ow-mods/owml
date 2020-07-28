using System;
using System.Collections.Generic;

namespace OWML.Common.Menus
{
    public interface IModMenu
    {
        event Action OnInit;

        Menu Menu { get; }

        List<IModButtonBase> BaseButtons { get; }
        List<IModButton> Buttons { get; }
        List<IModLayoutButton> LayoutButtons { get; }
        List<IModPromptButton> PromptButtons { get; }

        [Obsolete("Use GetTitleButton instead")]
        IModButton GetButton(string title);
        IModButton GetTitleButton(string title);
        IModPromptButton GetPromptButton(string title);

        [Obsolete("Use AddButton(IModButtonBase) instead.")]
        IModButton AddButton(IModButton button);
        [Obsolete("Use AddButton(IModButtonBase, int) instead.")]
        IModButton AddButton(IModButton button, int index);
        IModButtonBase AddButton(IModButtonBase button);
        IModButtonBase AddButton(IModButtonBase button, int index);

        List<IModToggleInput> ToggleInputs { get; }
        IModToggleInput GetToggleInput(string title);
        IModToggleInput AddToggleInput(IModToggleInput input);
        IModToggleInput AddToggleInput(IModToggleInput input, int index);

        List<IModSliderInput> SliderInputs { get; }
        IModSliderInput GetSliderInput(string title);
        IModSliderInput AddSliderInput(IModSliderInput input);
        IModSliderInput AddSliderInput(IModSliderInput input, int index);

        List<IModSelectorInput> SelectorInputs { get; }
        IModSelectorInput GetSelectorInput(string title);
        IModSelectorInput AddSelectorInput(IModSelectorInput input);
        IModSelectorInput AddSelectorInput(IModSelectorInput input, int index);

        List<IModTextInput> TextInputs { get; }
        IModTextInput GetTextInput(string title);
        IModTextInput AddTextInput(IModTextInput input);
        IModTextInput AddTextInput(IModTextInput input, int index);

        List<IModNumberInput> NumberInputs { get; }
        IModNumberInput GetNumberInput(string title);
        IModNumberInput AddNumberInput(IModNumberInput input);
        IModNumberInput AddNumberInput(IModNumberInput input, int index);

        List<IModSeparator> Separators { get; }
        IModSeparator AddSeparator(IModSeparator separator);
        IModSeparator AddSeparator(IModSeparator separator, int index);
        IModSeparator GetSeparator(string title);

        object GetInputValue(string key);
        void SetInputValue(string key, object value);

        void SelectFirst();
        void UpdateNavigation();
    }
}
