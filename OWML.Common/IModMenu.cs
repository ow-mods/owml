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
        IModButton CopyButton(string title);
        IModButton DuplicateButton(string title);
        IModButton ReplaceButton(string title);
        void AddButton(IModButton button);

        [Obsolete("Use button.Copy() and AddButton instead")]
        Button AddButton(string title, int index);

        Menu Menu { get; }

        int ButtonOffset { get; }
    }
}
