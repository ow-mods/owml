﻿namespace OWML.Common.Menus
{
    public interface IModTextInput : IModInput<string>
    {
        IModTextInput Copy();
        IModTextInput Copy(string key);
    }
}
