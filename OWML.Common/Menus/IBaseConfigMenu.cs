namespace OWML.Common.Menus
{
    public interface IBaseConfigMenu : IModPopupMenu
    {
        IModManifest Manifest { get; }

        void Initialize(Menu modMenuCopy, IModToggleInput toggleTemplate, IModSliderInput sliderTemplate, 
            IModTextInput textInputTemplate, IModNumberInput numberInputTemplate, IModComboInput comboInputTemplate);
    }
}
