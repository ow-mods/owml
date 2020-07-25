using OWML.Common;
using OWML.Logging;
using UnityEngine;

namespace OWML.ModHelper
{
    public class OwmlBehaviour : MonoBehaviour
    {
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void OnApplicationQuit()
        {
            OwmlConsole.WriteLine("", MessageType.Quit);
        }
    }
}
