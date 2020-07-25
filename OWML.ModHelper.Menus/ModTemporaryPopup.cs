using System;
using OWML.Common;
using OWML.Common.Menus;
using OWML.ModHelper.Events;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
    public abstract class ModTemporaryPopup : ModMenu
    {
        public ModTemporaryPopup(IModConsole console) : base(console) { }
        internal abstract void DestroySelf();
    }
}
