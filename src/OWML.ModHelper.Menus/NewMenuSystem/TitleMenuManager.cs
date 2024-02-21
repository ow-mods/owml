using OWML.Common;
using OWML.Utils;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace OWML.ModHelper.Menus.NewMenuSystem
{
	internal class TitleMenuManager : ITitleMenuManager
	{
		public SubmitAction CreateTitleButton(string text, int index, bool fromTop)
		{
			var titleScreenManager = Object.FindObjectOfType<TitleScreenManager>();

			var existingTitleButton = titleScreenManager.GetValue<SubmitAction>("_resetGameAction");
			var newButton = Object.Instantiate(existingTitleButton.gameObject);
			newButton.transform.parent = existingTitleButton.transform.parent;
			newButton.transform.localScale = existingTitleButton.transform.localScale;
			newButton.name = $"Button-{text}";

			Object.Destroy(newButton.GetComponent<SubmitActionConfirm>());
			var submitAction = newButton.AddComponent<SubmitAction>();

			SetButtonIndex(submitAction, index, fromTop);
			SetButtonText(submitAction, text);

			newButton.GetComponent<CanvasGroup>().alpha = 0;
			var animController = GameObject.Find("TitleMenuManagers").GetComponent<TitleAnimationController>();
			var list = animController._buttonFadeControllers.ToList();
			list.Insert(newButton.transform.GetSiblingIndex() - 2, new CanvasGroupFadeController
			{
				group = newButton.GetComponent<CanvasGroup>()
			});
			animController._buttonFadeControllers = list.ToArray();

			titleScreenManager._mainMenuTextFields = titleScreenManager._mainMenuTextFields.Append(submitAction.GetComponentInChildren<Text>()).ToArray();

			return submitAction;
		}

		public void SetButtonText(SubmitAction buttonAction,  string text)
		{
			buttonAction.GetComponentInChildren<Text>().text = text;
		}

		public void SetButtonIndex(SubmitAction buttonAction, int index, bool fromTop)
		{
			if (index > buttonAction.transform.parent.childCount - 4)
			{
				throw new IndexOutOfRangeException("Can't set a button to have a higher index than being last!");
			}

			var indexToSetTo = fromTop
				? index + 2
				: buttonAction.transform.parent.childCount - 3 - index;

			buttonAction.transform.SetSiblingIndex(indexToSetTo);
		}
	}
}
