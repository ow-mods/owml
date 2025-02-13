namespace OWML.Common
{
	public delegate void PopupConfirmEvent();
	public delegate bool PopupValidateEvent();
	public delegate void PopupCancelEvent();

	public interface IOWMLThreeChoicePopupMenu
	{
		event PopupConfirmEvent OnPopupConfirm1;
		event PopupConfirmEvent OnPopupConfirm2;
		event PopupValidateEvent OnPopupValidate;
		event PopupCancelEvent OnPopupCancel;

		void EnableMenu(bool value);
		void SetText(string message, string confirm1Text, string confirm2Text, string cancelText);

		public event Menu.ActivateMenuEvent OnActivateMenu;
		public event Menu.DeactivateMenuEvent OnDeactivateMenu;
	}
}
