using System;

namespace OWML.Common.Interfaces
{
    public interface IModSceneEvents
    {
        event Action<OWScene, OWScene> OnStartSceneChange;
        event Action<OWScene, OWScene> OnCompleteSceneChange;
    }
}
