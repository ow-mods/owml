namespace OWML.Common.Interfaces.Menus
{
	public interface IOWMLKeyRebindingElement
	{
		void Initialize(SettingsMenuModel settingsMenuModel);
		void UpdateDisplay(bool forceRefresh = false);
		RebindableID GetRebindableID();
	}
}
