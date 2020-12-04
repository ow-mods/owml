using System.Collections.ObjectModel;
using UnityEngine;

namespace OWML.Common.Interfaces
{
    public interface IModInputCombination
    {
        float LastPressedMoment { get; }
        float PressDuration { get; }
        string ModName { get; }
        string Name { get; }
        string FullName { get; }
        ReadOnlyCollection<KeyCode> Singles { get; }
        ReadOnlyCollection<long> Hashes { get; }
        bool IsFirst { get; }

        void InternalSetPressed(bool isPressed = true);
    }
}
