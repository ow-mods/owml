using OWML.Common.Interfaces;
using OWML.Common.Menus;
using System;

namespace OWML.Common
{
	public interface IModHelper
	{
		IModLogger Logger { get; }

		IModConsole Console { get; }

		IModEvents Events { get; }

		IHarmonyHelper HarmonyHelper { get; }

		IModAssets Assets { get; }

		IModStorage Storage { get; }

		[Obsolete("Use the new menu system instead.")]
		IModMenus Menus { get; }

		IModManifest Manifest { get; }

		IModConfig Config { get; }

		IModDefaultConfig DefaultConfig { get; }

		IOwmlConfig OwmlConfig { get; }

		IModInteraction Interaction { get; }

		IMenuManager MenuHelper { get; }

		IModTranslations MenuTranslations { get; }
	}
}
