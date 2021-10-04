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
			var button = element.GetComponentInChildren<Button>(); // todo
			Button = new ModTitleButton(button, menu);
			Subscribe(Button);
			PopupManager = popupManager;
		}
	}
}
