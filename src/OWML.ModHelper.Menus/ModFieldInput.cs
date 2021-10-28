using System.Linq;
using OWML.Common.Menus;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
	public abstract class ModFieldInput<T> : ModPopupInput<T>, IModFieldInput<T>
	{
		public IModButton Button { get; }

		protected readonly IModPopupManager PopupManager;

		protected ModFieldInput(OptionsSelectorElement element, IModMenu menu, IModPopupManager popupManager)
			: base(element, menu)
		{
			PopupManager = popupManager;

			var buttons = element.GetComponentsInChildren<Button>().ToList();

			var leftButton = buttons.First();
			leftButton.GetComponentsInChildren<Image>().ToList().ForEach(x => x.gameObject.SetActive(false));

			var center = leftButton.transform.parent.parent.GetChild(1);
			var centerRect = center.GetComponent<RectTransform>().rect;

			var rightButton = buttons.Last();
			rightButton.transform.position = center.transform.position;
			rightButton.GetComponent<RectTransform>().sizeDelta = centerRect.size;
			rightButton.transform.SetParent(center);
			rightButton.GetComponentsInChildren<Image>().ToList().ForEach(x => x.gameObject.SetActive(false));

			Button = new ModTitleButton(rightButton, menu);
			Subscribe(Button);

			var text = center.GetComponentInChildren<Text>();
			OnChange += value => text.text = value.ToString();
		}

		public void Initialize(string value)
		{
			SelectorElement.Initialize(0, new string[] { value });
		}
	}
}
