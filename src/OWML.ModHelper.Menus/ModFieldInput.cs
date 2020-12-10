using OWML.Common.Interfaces.Menus;
using OWML.Utils;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public abstract class ModFieldInput<T> : ModPopupInput<T>, IModFieldInput<T>
    {
        public IModButton Button { get; }

        protected readonly IModPopupManager PopupManager;

        protected ModFieldInput(TwoButtonToggleElement toggle, IModMenu menu, IModPopupManager popupManager) : base(toggle, menu)
        {
            Button = new ModTitleButton(toggle.GetValue<Button>("_buttonTrue"), menu);
            Subscribe(Button);
            PopupManager = popupManager;
        }
    }
}
