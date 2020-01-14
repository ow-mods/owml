using OWML.Common;
using UnityEngine;

namespace OWML.ModHelper
{
    public class OwmlBehaviour : MonoBehaviour
    {
        void OnApplicationQuit()
        {
            ModConsole.Instance.WriteLine(Constants.QuitKeyPhrase);
        }
    }
}
