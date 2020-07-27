using OWML.Common;
using OWML.Common.Menus;
using OWML.ModHelper.Events;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public abstract class ModFieldInput<T> : ModPopupInput<T>, IModFieldInput<T>
    {
        public IModButton Button { get; }

        protected readonly IModInputMenu InputMenu;

        protected ModFieldInput(TwoButtonToggleElement toggle, IModMenu menu, IModInputMenu inputMenu, IModEvents events) 
            : base(toggle, menu, events)
        {
            Button = new ModTitleButton(toggle.GetValue<Button>("_buttonTrue"), menu);
            Subscribe(Button);
            InputMenu = inputMenu;
        }
    }
}
