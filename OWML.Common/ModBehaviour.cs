using UnityEngine;

namespace OWML.Common
{
    public class ModBehaviour : MonoBehaviour
    {
        public static IModHelper ModHelper;

        public void Init(IModHelper modHelper)
        {
            ModHelper = modHelper;
            DontDestroyOnLoad(gameObject);
        }

    }
}
