using OWML.Common;
using UnityEngine;

namespace OWML.ModHelper
{
    public class ModBehaviour : MonoBehaviour, IModBehaviour
    {
        public static IModHelper ModHelper;

        public void Init(IModHelper modHelper)
        {
            ModHelper = modHelper;
            DontDestroyOnLoad(gameObject);
        }

    }
}
