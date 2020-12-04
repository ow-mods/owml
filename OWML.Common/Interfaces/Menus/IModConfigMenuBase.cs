namespace OWML.Common.Interfaces.Menus
{
    public interface IModConfigMenuBase : IModPopupMenu
    {
        IModManifest Manifest { get; }

        void Initialize(Menu modMenuCopy, IModToggleInput toggleTemplate, IModSliderInput sliderTemplate, 
            IModTextInput textInputTemplate, IModNumberInput numberInputTemplate,
            IModComboInput comboInputTemplate, IModSelectorInput selectorTemplate);
    }
}
