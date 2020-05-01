namespace OWML.Common.Menus
{
    public interface IModConfigMenu : IModPopupMenu
    {
        IModData ModData { get; }
        IModBehaviour Mod { get; }

        void Initialize(Menu modMenuCopy, IModToggleInput toggleTemplate, IModSliderInput sliderTemplate, IModTextInput textInputTemplate, IModNumberInput numberInputTemplate, IModComboInput comboInputTemplate);
    }
}
