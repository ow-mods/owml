using OWML.Common;
using OWML.Utils;

namespace OWML.ModHelper.Menus
{
	public class OwmlConfigMenu : ModConfigMenuBase
	{
		private const string DebugModeTitle = "Debug mode";
		private const string ForceExeTitle = "Force run through .exe";

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
			AddConfigInput(ForceExeTitle, _config.ForceExe, 3);
			UpdateNavigation();
			SelectFirst();
		}

		public override void UpdateUIValues()
		{
			GetToggleInput(DebugModeTitle).Value = _config.DebugMode;
			GetToggleInput(ForceExeTitle).Value = _config.ForceExe;
		}

		protected override void OnSave()
		{
			_config.DebugMode = GetInputValue<bool>(DebugModeTitle);
			_config.ForceExe = GetInputValue<bool>(ForceExeTitle);
			JsonHelper.SaveJsonObject($"{_config.OWMLPath}{Constants.OwmlConfigFileName}", _config);
			Close();
		}

		protected override void OnReset()
		{
			_config.GamePath = _defaultConfig.GamePath;
			_config.DebugMode = _defaultConfig.DebugMode;
			_config.ForceExe = _defaultConfig.ForceExe;
			UpdateUIValues();
		}
	}
}
