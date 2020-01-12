using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace OWML.Common
{
    public interface IModMenu
    {
        [Obsolete("Use Buttons instead")]
        List<Button> GetButtons();
        List<IModButton> Buttons { get; }
        IModButton GetButton(string title);

        [Obsolete("Use button.Copy() and AddButton instead")]
        Button AddButton(string title, int index);
        void AddButton(IModButton button);

        LayoutGroup LayoutGroup { get; }
        Menu Menu { get; }

        int ButtonOffset { get; }
    }
}
