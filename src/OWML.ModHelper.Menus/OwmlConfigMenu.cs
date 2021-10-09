using OWML.Common;
using OWML.Utils;

namespace OWML.ModHelper.Menus
{
	public class OwmlConfigMenu : ModConfigMenuBase
	{
		private const string DebugModeTitle = "Debug mode";

		private readonly IOwmlConfig _config;
		private readonly IOwmlConfig _defaultConfig;

		public OwmlConfigMenu(
			IModManifest manifest,
			IOwmlConfig config,
			IModStorage storage,
			IApplicationHelper appHelper,
			IModConsole console)
				: base(manifest, storage, console)
		{
			_config = config;
			_defaultConfig = JsonHelper.LoadJsonObject<OwmlConfig>($"{appHelper.DataPath}/Managed/{Constants.OwmlDefaultConfigFileName}");
		}

		protected override void AddInputs()
		{
			AddConfigInput(DebugModeTitle, _config.DebugMode, 2);
			UpdateNavigation();
			SelectFirst();
		}

		public override void UpdateUIValues()
		{
			GetToggleInput(DebugModeTitle).Value = _config.DebugMode;
		}

		protected override void OnSave()
		{
			_config.DebugMode = GetInputValue<bool>(DebugModeTitle);
			Storage.Save(_config, Constants.OwmlConfigFileName);
			Close();
		}

		protected override void OnReset()
		{
			_config.GamePath = _defaultConfig.GamePath;
			_config.DebugMode = _defaultConfig.DebugMode;
			_config.BlockInput = _defaultConfig.BlockInput;
			UpdateUIValues();
		}
	}
}
