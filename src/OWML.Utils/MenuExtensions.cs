using UnityEngine;

namespace OWML.Utils
{
	public static class MenuExtensions
	{
		public static void SetMessage(this PopupMenu menu, string message) => menu._labelText.text = message;

		public static void SetButtonVisible(this SubmitAction buttonAction, bool visible)
		{
			var activeAlpha = 1;

			if (LoadManager.GetCurrentScene() == OWScene.TitleScreen)
			{
				var titleAnimationController = Resources.FindObjectsOfTypeAll<TitleScreenManager>()[0]._gfxController;
				activeAlpha = titleAnimationController.IsTitleAnimationComplete() ? 1 : 0;
				if (titleAnimationController.IsFadingInMenuOptions())
				{
					activeAlpha = 1;
				}
			}

			buttonAction.gameObject.SetActive(visible);
			buttonAction.GetComponent<CanvasGroup>().alpha = visible ? activeAlpha : 0;
		}
	}
}
