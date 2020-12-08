using OWML.Common.Enums;
using OWML.Logging;
using UnityEngine;

namespace OWML.ModHelper
{
    public class OwmlBehaviour : MonoBehaviour
    {
        public void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void OnApplicationQuit()
        {
            ModConsole.OwmlConsole.WriteLine("", MessageType.Quit);
        }
    }
}
