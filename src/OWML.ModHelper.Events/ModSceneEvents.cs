using System;
using OWML.Common.Interfaces;

namespace OWML.ModHelper.Events
{
    public class ModSceneEvents : IModSceneEvents
    {
        public event Action<OWScene, OWScene> OnStartSceneChange;
        public event Action<OWScene, OWScene> OnCompleteSceneChange;

        public ModSceneEvents()
        {
            LoadManager.OnStartSceneLoad += OnStartSceneLoad;
            LoadManager.OnCompleteSceneLoad += OnCompleteSceneLoad;
        }

        private void OnStartSceneLoad(OWScene originalScene, OWScene newScene)
        {
            OnStartSceneChange?.Invoke(originalScene, newScene);
        }

        private void OnCompleteSceneLoad(OWScene originalScene, OWScene newScene)
        {
            OnCompleteSceneChange?.Invoke(originalScene, newScene);
        }

    }
}
