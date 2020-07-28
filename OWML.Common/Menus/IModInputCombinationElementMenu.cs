using System;

namespace OWML.Common.Menus
{
    public interface IModInputCombinationElementMenu : IModMenu
    {
        event Action<string> OnConfirm;
        event Action OnCancel;
    }
}
