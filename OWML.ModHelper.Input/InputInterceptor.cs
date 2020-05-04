using UnityEngine;

namespace OWML.ModHelper.Input
{
    internal class InputInterceptor
    {
        public static void SingleAxisRemovePre(SingleAxisCommand __instance)
        {
            ModInputHandler.Instance.UnregisterGamesBinding(__instance);
        }

        public static void DoubleAxisRemovePre(DoubleAxisCommand __instance)
        {
            ModInputHandler.Instance.UnregisterGamesBinding(__instance);
        }

        public static void SingleAxisUpdatePost(
            SingleAxisCommand __instance,
            ref float ____value,
            int ____axisDirection,
            KeyCode ____gamepadKeyCodePositive,
            KeyCode ____gamepadKeyCodeNegative,
            KeyCode ____keyPositive,
            KeyCode ____keyNegative
        )
        {
            KeyCode positiveKey, negativeKey;
            ModInputHandler.Instance.RegisterGamesBinding(__instance);
            int axisDirection = 1;
            if (OWInput.UsingGamepad())
            {
                axisDirection = ____axisDirection;
                positiveKey = ____gamepadKeyCodePositive;
                negativeKey = ____gamepadKeyCodeNegative;
            }
            else
            {
                positiveKey = ____keyPositive;
                negativeKey = ____keyNegative;
            }
            if (UnityEngine.Input.GetKey(positiveKey) && ModInputHandler.Instance.ShouldIgnore(positiveKey))
            {
                ____value -= 1f * axisDirection;
            }
            if (UnityEngine.Input.GetKey(negativeKey) && ModInputHandler.Instance.ShouldIgnore(negativeKey))
            {
                ____value += 1f * axisDirection;
            }
        }

        public static void DoubleAxisUpdatePost(
            DoubleAxisCommand __instance,
            ref Vector2 ____value,
            KeyCode ____keyboardXPos,
            KeyCode ____keyboardYPos,
            KeyCode ____keyboardXNeg,
            KeyCode ____keyboardYNeg
        )
        {
            ModInputHandler.Instance.RegisterGamesBinding(__instance);
            if (OWInput.UsingGamepad())
            {
                return;
            }
            if (UnityEngine.Input.GetKey(____keyboardXPos) && ModInputHandler.Instance.ShouldIgnore(____keyboardXPos))
            {
                ____value.x -= 1f;
            }
            if (UnityEngine.Input.GetKey(____keyboardXNeg) && ModInputHandler.Instance.ShouldIgnore(____keyboardXNeg))
            {
                ____value.x += 1f;
            }
            if (UnityEngine.Input.GetKey(____keyboardYPos) && ModInputHandler.Instance.ShouldIgnore(____keyboardYPos))
            {
                ____value.y -= 1f;
            }
            if (UnityEngine.Input.GetKey(____keyboardYNeg) && ModInputHandler.Instance.ShouldIgnore(____keyboardYNeg))
            {
                ____value.y += 1f;
            }

        }
    }
}
