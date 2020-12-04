using System;

namespace OWML.Common.Interfaces.Menus
{
    public interface IModInputCombinationElementMenu : IModMenu
    {
        event Action<string> OnConfirm;
        event Action OnCancel;
    }
}
