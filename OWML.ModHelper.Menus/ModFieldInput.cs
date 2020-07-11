using OWML.Common.Menus;
using OWML.ModHelper.Events;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public abstract class ModFieldInput<T> : ModPopupInput<T>, IModFieldInput<T>
    {
        public IModTitleButton Button { get; }

        protected readonly IModInputMenu InputMenu;

        protected ModFieldInput(TwoButtonToggleElement toggle, IModMenu menu, IModInputMenu inputMenu) : base(toggle, menu)
        {
            Button = new ModTitleButton(toggle.GetValue<Button>("_buttonTrue"), menu);
            Subscribe(Button);
            InputMenu = inputMenu;
        }
    }
}
