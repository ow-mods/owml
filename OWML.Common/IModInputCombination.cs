using System;
using System.Collections.ObjectModel;
using UnityEngine;

namespace OWML.Common
{
    public interface IModInputCombination
    {
        float LastPressedMoment { get; }
        float PressDuration { get; }
        string ModName { get; }
        string Name { get; }
        ReadOnlyCollection<KeyCode> Singles { get; }
        ReadOnlyCollection<long> Hashes { get; }
        bool IsFirst { get; }
        [Obsolete("For internal use")]
        void SetPressed(bool isPressed = true);
    }
}
