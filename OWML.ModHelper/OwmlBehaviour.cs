using OWML.Common;
using OWML.ModHelper.Logging;
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
            ModConsole.Instance.WriteLine(Constants.QuitKeyPhrase);
        }
    }
}
