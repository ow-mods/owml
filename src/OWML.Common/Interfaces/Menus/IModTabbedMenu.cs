﻿namespace OWML.Common.Interfaces.Menus
{
    public interface IModTabbedMenu : IModPopupMenu
    {
        IModTabMenu GameplayTab { get; }
        IModTabMenu AudioTab { get; }
        IModTabMenu InputTab { get; }
        IModTabMenu GraphicsTab { get; }

        IModButton RebindingButton { get; }
        IModPopupMenu RebindingMenu { get; }

        new IModTabbedMenu Copy();

        void Initialize(TabbedMenu menu);
        new TabbedMenu Menu { get; }
        void AddTab(IModTabMenu tab);
    }
}