using UnityEngine;

namespace OWML.Common
{
    public class ModBehaviour : MonoBehaviour
    {
        protected IModHelper ModHelper;

        public void Init(IModHelper modHelper)
        {
            ModHelper = modHelper;
            DontDestroyOnLoad(gameObject);
        }

    }
}
