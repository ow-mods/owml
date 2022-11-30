using OWML.Common.Menus;

namespace OWML.Common
{
	public interface IModHelper
	{
		IModConsole Console { get; }

		IModEvents Events { get; }

		IHarmonyHelper HarmonyHelper { get; }

		IModAssets Assets { get; }

		IModStorage Storage { get; }

		IModMenus Menus { get; }

		IModManifest Manifest { get; }

		IModConfig Config { get; }

		IOwmlConfig OwmlConfig { get; }

		IModInteraction Interaction { get; }
	}
}
