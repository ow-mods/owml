namespace OWML.Common.Menus
{
    public interface IModConfigMenu : IModPopupMenu
    {
        IModManifest Manifest { get; }
        IModConfig Config { get; }
        IModConfig DefaultConfig { get; }
        IModBehaviour Mod { get; }

        void Initialize(Menu modMenuCopy, IModToggleInput toggleTemplate, IModSliderInput sliderTemplate, 
            IModTextInput textInputTemplate, IModNumberInput numberInputTemplate, IModComboInput comboInputTemplate);
    }
}
