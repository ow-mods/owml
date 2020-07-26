using OWML.Common;
using UnityEngine;

namespace OWML.ModHelper
{
    public class OwmlBehaviour : MonoBehaviour
    {
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            gameObject.AddComponent<ModTaskDelayer>();
        }

        private void OnApplicationQuit()
        {
            ModConsole.Instance.WriteLine("", MessageType.Quit);
        }
    }
}
