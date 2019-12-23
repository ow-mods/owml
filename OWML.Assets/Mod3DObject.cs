using UnityEngine;

namespace OWML.Assets
{
    public class Mod3DObject : MonoBehaviour
    {
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

    }
}
