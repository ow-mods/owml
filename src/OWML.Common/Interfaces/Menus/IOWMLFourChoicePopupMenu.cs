namespace OWML.Common
{
	public interface IOWMLFourChoicePopupMenu
	{
		event PopupConfirmEvent OnPopupConfirm1;
		event PopupConfirmEvent OnPopupConfirm2;
		event PopupConfirmEvent OnPopupConfirm3;
		event PopupValidateEvent OnPopupValidate;
		event PopupCancelEvent OnPopupCancel;

		void EnableMenu(bool value);
		void SetText(string message, string confirm1Text, string confirm2Text, string confirm3Text, string cancelText);

		public event Menu.ActivateMenuEvent OnActivateMenu;
		public event Menu.DeactivateMenuEvent OnDeactivateMenu;
	}
}
