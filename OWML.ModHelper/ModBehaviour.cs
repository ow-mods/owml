using OWML.Common;
using UnityEngine;

namespace OWML.ModHelper
{
    public class ModBehaviour : MonoBehaviour, IModBehaviour
    {
        public static IModHelper ModHelper;
        public IModManifest ModManifest { get; set; }

        public void Init(IModHelper modHelper, IModManifest manifest)
        {
            ModHelper = modHelper;
            ModManifest = manifest;
            DontDestroyOnLoad(gameObject);
        }

    }
}
