using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OWML.Common.Menus
{
    public interface IModInputCombinationMenu : IModPopupMenu
    {
        event Action OnCancel;
        event Action<string> OnConfirm;
        List<IModInputCombinationElement> CombinationElements { get; }
        string Combination { get; set; }
        void Initialize(Menu menu, IModInputCombinationElement combinationElementTemplate);
    }
}
