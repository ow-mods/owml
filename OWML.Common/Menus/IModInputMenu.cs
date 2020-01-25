using System;

namespace OWML.Common.Menus
{
    public interface IModInputMenu : IModMenu
    {
        event Action<string> OnInput;
        void Initialize(PopupInputMenu inputMenu);
        void Open();
        void Open(string placeholderText);
    }
}
