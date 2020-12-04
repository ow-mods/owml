using System;

namespace OWML.Common.Interfaces
{
    public interface IModPlayerEvents
    {
        event Action<PlayerBody> OnPlayerAwake;
    }
}
