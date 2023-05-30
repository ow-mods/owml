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

		IModManifest Manifest { get; }

		IModConfig Config { get; }

		IOwmlConfig OwmlConfig { get; }

		IModInteraction Interaction { get; }
	}
}
