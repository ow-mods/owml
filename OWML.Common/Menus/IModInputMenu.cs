using System;
using UnityEngine.UI;

namespace OWML.Common.Menus
{
    public interface IModInputMenu : IModMenu
    {
        event Action<string> OnInput;
        void Initialize(PopupInputMenu inputMenu);
        void Open(InputField.ContentType contentType, InputField.CharacterValidation validation, string value);
    }
}
