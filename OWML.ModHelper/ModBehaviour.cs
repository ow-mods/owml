using OWML.Common;
using UnityEngine;

namespace OWML.ModHelper
{
    public class ModBehaviour : MonoBehaviour
    {
        public IModHelper ModHelper { get; private set; }

        public void Init(IModHelper modHelper)
        {
            ModHelper = modHelper;
            DontDestroyOnLoad(gameObject);
        }
    }
}
