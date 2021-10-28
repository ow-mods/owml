namespace OWML.Common.Menus
{
	public interface IModConfigMenuBase : IModPopupMenu
	{
		IModManifest Manifest { get; }

		void UpdateUIValues();

		void Initialize(
			Menu modMenuCopy,
			IModToggleInput toggleTemplate,
			IModSliderInput sliderTemplate,
			IModTextInput textInputTemplate,
			IModNumberInput numberInputTemplate,
			IModSelectorInput selectorTemplate,
			IModSeparator seperatorTemplate);
	}
}
