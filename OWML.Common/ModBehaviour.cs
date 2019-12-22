using UnityEngine;

namespace OWML.Common
{
    public class ModBehaviour : MonoBehaviour
    {
        public static IModHelper ModHelper;
        public IModManifest ModManifest;

        public void Init(IModHelper modHelper, IModManifest manifest)
        {
            ModHelper = modHelper;
            ModManifest = manifest;
            DontDestroyOnLoad(gameObject);
        }

    }
}
