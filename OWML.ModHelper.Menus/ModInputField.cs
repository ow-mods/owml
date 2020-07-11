using OWML.Common.Menus;
using OWML.ModHelper.Events;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public abstract class ModInputField<T> : ModMenuInput<T>, IModInputField<T>
    {
        public IModTitleButton Button { get; }

        protected readonly IModInputMenu InputMenu;

        protected ModInputField(TwoButtonToggleElement toggle, IModMenu menu, IModInputMenu inputMenu) : base(toggle, menu)
        {
            Button = new ModTitleButton(toggle.GetValue<Button>("_buttonTrue"), menu);
            Subscribe(Button);
            InputMenu = inputMenu;
        }
    }
}
