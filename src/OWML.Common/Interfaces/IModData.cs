namespace OWML.Common
{
	public interface IModData
	{
		IModManifest Manifest { get; }

		IModConfig Config { get; }

		IModConfig DefaultConfig { get; }

		IModStorage Storage { get; }

		bool Enabled { get; }

		bool RequireReload { get; }

		void UpdateSnapshot();

		void ResetConfigToDefaults();

		bool FixConfigs();
	}
}