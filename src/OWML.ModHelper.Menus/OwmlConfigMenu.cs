using OWML.Common;
using OWML.Utils;

namespace OWML.ModHelper.Menus
{
	public class OwmlConfigMenu : ModConfigMenuBase
	{
		private const string DebugModeTitle = "Debug mode";
		private const string BlockInputTitle = "Mod inputs can block game actions";

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
			AddConfigInput(BlockInputTitle, _config.BlockInput, 3);
			UpdateNavigation();
			SelectFirst();
		}

		protected override void UpdateUIValues()
		{
			GetToggleInput(DebugModeTitle).Value = _config.DebugMode;
			GetToggleInput(BlockInputTitle).Value = _config.BlockInput;
		}

		protected override void OnSave()
		{
			_config.DebugMode = GetInputValue<bool>(DebugModeTitle);
			_config.BlockInput = GetInputValue<bool>(BlockInputTitle);
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
