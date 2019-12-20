using UnityEngine;

namespace OWML.Common
{
    public class ModBehaviour : MonoBehaviour
    {
        public static IModHelper ModHelper;

        public void Init(IModHelper modHelper)
        {
            ModHelper = modHelper;
            DontDestroyOnLoad(gameObject);
            if (modHelper.Config.Verbose)
            {
                Application.logMessageReceived += OnLogMessageReceived;
            }
        }

        private void OnLogMessageReceived(string message, string stackTrace, LogType type)
        {
            ModHelper.Logger.Log($"{type}: {message}");
            if (type == LogType.Error || type == LogType.Exception)
            {
                ModHelper.Console.WriteLine($"{type}: {message}");
            }
        }

    }
}
