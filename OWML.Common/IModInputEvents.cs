using System;

namespace OWML.Common
{
    public interface IModInputEvents
    {
        event Action<SingleAxisCommand> OnNewlyPressed;
        event Action<SingleAxisCommand> OnNewlyReleased;
        event Action<SingleAxisCommand> OnNewlyHeld;
        event Action<SingleAxisCommand> OnPressed;
        event Action<SingleAxisCommand> OnTapped;
        event Action<SingleAxisCommand> OnHeld;

        void AddToListener(SingleAxisCommand command);
        void RemoveFromListener(SingleAxisCommand command);
        void BlockNextRelease();
    }
}
