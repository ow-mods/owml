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
            Output.GetOwmlOutput().WriteLine(Constants.QuitKeyPhrase);
        }
    }
}
