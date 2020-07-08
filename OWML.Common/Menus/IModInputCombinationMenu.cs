using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace OWML.Common.Menus
{
    public interface IModInputCombinationMenu : IModPopupMenu
    {
        event Action OnCancel;
        event Action<string> OnConfirm;
        List<IModInputCombinationElement> CombinationElements { get; }
        string GenerateCombination();
        void FillMenu(string combination);
        void Initialize(Menu menu, IModInputCombinationElement combinationElementTemplate);
        void RemoveCombinationElement(IModInputCombinationElement element);
    }
}
