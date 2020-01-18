using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace OWML.Common.Menus
{
    public interface IModMenu
    {
        event Action OnInit;

        Menu Menu { get; }
        List<IModButton> Buttons { get; }
        IModButton GetButton(string title);
        void AddButton(IModButton button);
        void AddButton(IModButton button, int index);

        [Obsolete("Use Buttons instead")]
        List<Button> GetButtons();
        [Obsolete("Use button.Duplicate instead")]
        Button AddButton(string title, int index);
    }
}
