using System;
using System.Collections.Generic;

namespace OWML.Common.Menus
{
    public interface IModInputCombinationMenu : IModPopupMenu
    {
        event Action<string> OnConfirm;
        event Action OnCancel;

        List<IModInputCombinationElement> CombinationElements { get; }
        string GenerateCombination();
        void FillMenu(string combination);
        void Initialize(Menu menu, IModInputCombinationElement combinationElementTemplate);
        void RemoveCombinationElement(IModInputCombinationElement element);
    }
}
