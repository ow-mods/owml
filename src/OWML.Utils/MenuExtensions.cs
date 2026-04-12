using OWML.Common;
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

		public static RectOffset GetLabelPadding(this MenuSide side)
		{
			return side switch
			{
				MenuSide.LEFT => new RectOffset(20, 180, 0, 0),
				MenuSide.CENTER => new RectOffset(100, 100, 0, 0),
				MenuSide.RIGHT => new RectOffset(180, 20, 0, 0),
				_ => new RectOffset(100, 100, 0, 0)
			};
		}

		public static TextAnchor GetTextAnchor(this MenuSide side)
		{
			return side switch
			{
				MenuSide.LEFT => TextAnchor.MiddleLeft,
				MenuSide.CENTER => TextAnchor.MiddleCenter,
				MenuSide.RIGHT => TextAnchor.MiddleRight,
				_ => TextAnchor.MiddleCenter
			};
		}
	}
}
