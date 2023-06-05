namespace OWML.Common.Menus
{
	public interface IModPopupManager
	{
		void Initialize(PopupInputMenu inputMenu, IModTabbedMenu options);

		IModMessagePopup CreateMessagePopup(string message, bool addCancel = false, string okMessage = "OK", string cancelMessage = "Cancel");

		IModInputMenu CreateInputPopup(InputType inputType, string value);
	}
}
