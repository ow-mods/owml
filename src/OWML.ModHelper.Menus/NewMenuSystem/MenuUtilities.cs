using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus.NewMenuSystem
{
	internal static class MenuUtilities
	{
		internal static void AddToLangController(Text textComponent)
		{
			var controllers = Resources.FindObjectsOfTypeAll<FontAndLanguageController>();

			if (controllers.Length == 0)
			{
				return;
			}

			var controller = controllers.FirstOrDefault(x => x.isActiveAndEnabled);

			if (controller == null)
			{
				return;
			}

			controller.AddTextElement(textComponent, false);
		}
	}
}
