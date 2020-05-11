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
            ModOutput.OwmlOutput.WriteLine(Constants.QuitKeyPhrase);
        }
    }
}
