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
        List<IModInput<bool>> ToggleElements { get; }
        List<IModInput<float>> SliderElements { get; }
        IModButton GetButton(string title);
        IModButton AddButton(IModButton button);
        IModButton AddButton(IModButton button, int index);

        [Obsolete("Use Buttons instead")]
        List<Button> GetButtons();
        [Obsolete("Use button.Duplicate instead")]
        Button AddButton(string title, int index);
    }
}
