using System.Linq;
using OWML.Common.Menus;
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

			var button = element.GetComponentsInChildren<Button>().ToList().First(); // todo
			Button = new ModTitleButton(button, menu);
			Subscribe(Button);

			var center = button.transform.parent.parent.GetChild(1);
			var text = center.GetComponentInChildren<Text>();
			OnChange += value => text.text = value.ToString();
		}
	}
}
