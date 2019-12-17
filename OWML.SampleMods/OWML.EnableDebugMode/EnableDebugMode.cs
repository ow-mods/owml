using OWML.Common;
using OWML.Events;

namespace OWML.EnableDebugMode
{
    public class EnableDebugMode : ModBehaviour
    {
        private void Start()
        {
            ModHelper.Console.WriteLine($"In {nameof(EnableDebugMode)}!");
            ModHelper.HarmonyHelper.EmptyMethod<DebugInputManager>("Awake");
        }

        private int _renderValue;

        private void Update()
        {
            if (Input.GetKeyDown(DebugKeyCode.cycleGUIMode))
            {
                ModHelper.Console.WriteLine("F1 pressed!");
                _renderValue++;
                if (_renderValue >= 8)
                {
                    _renderValue = 0;
                }
                ModHelper.Console.WriteLine("_renderValue: " + _renderValue);
                typeof(GUIMode).GetAnyField("_renderMode").SetValue(null, _renderValue);
            }
        }

    }
}
