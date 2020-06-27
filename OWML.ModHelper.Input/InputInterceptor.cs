using UnityEngine;
using System.Collections.Generic;

namespace OWML.ModHelper.Input
{
    internal class InputInterceptor
    {
        public static void SingleAxisUpdatePost(
            SingleAxisCommand __instance,
            ref float ____value,
            List<KeyCode> ____posKeyCodes,
            List<KeyCode> ____negKeyCodes
        )
        {
            foreach (var key in ____posKeyCodes)
            {
                if (ModInputHandler.Instance.IsPressedAndIgnored(key))
                {
                    ____value = 0f;
                }
            }
            foreach (var key in ____negKeyCodes)
            {
                if (ModInputHandler.Instance.IsPressedAndIgnored(key))
                {
                    ____value = 0f;
                }
            }
        }

        public static void DoubleAxisUpdatePost(
            DoubleAxisCommand __instance,
            ref Vector2 ____value,
            List<KeyCode> ____posXKeyCodes,
            List<KeyCode> ____negXKeyCodes,
            List<KeyCode> ____posYKeyCodes,
            List<KeyCode> ____negYKeyCodes
        )
        {
            if (OWInput.UsingGamepad())
            {
                return;
            }
            foreach (var key in ____posXKeyCodes)
            {
                if (ModInputHandler.Instance.IsPressedAndIgnored(key))
                {
                    ____value.x = 0f;
                }
            }
            foreach (var key in ____negXKeyCodes)
            {
                if (ModInputHandler.Instance.IsPressedAndIgnored(key))
                {
                    ____value.x = 0f;
                }
            }
            foreach (var key in ____posYKeyCodes)
            {
                if (ModInputHandler.Instance.IsPressedAndIgnored(key))
                {
                    ____value.y = 0f;
                }
            }
            foreach (var key in ____negYKeyCodes)
            {
                if (ModInputHandler.Instance.IsPressedAndIgnored(key))
                {
                    ____value.y = 0f;
                }
            }
        }
    }
}
