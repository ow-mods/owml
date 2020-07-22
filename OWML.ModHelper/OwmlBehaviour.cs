using OWML.Common;
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
            ModConsole.Instance.WriteLine(MessageType.Quit, "");
        }
    }
}
