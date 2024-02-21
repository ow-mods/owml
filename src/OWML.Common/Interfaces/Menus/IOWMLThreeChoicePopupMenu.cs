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
	}
}
