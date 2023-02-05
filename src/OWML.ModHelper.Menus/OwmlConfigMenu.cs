using OWML.Common;
using OWML.Utils;

namespace OWML.ModHelper.Menus
{
	public class OwmlConfigMenu : ModConfigMenuBase
	{
		private const string DebugModeTitle = "Debug mode";
		private const string DebugModeTooltip = "Enable verbose logging. Some effects only enable/disable when game is reloaded.";
		private const string ForceExeTitle = "Force run through .exe";
		private const string ForceExeTooltip = "Force OWML to run the game's exe, rather than going through Steam/Epic.";
		private const string IncrementalGCTitle = "Enable incremental GC";
		private const string IncrementalGCTooltip = "Incremental GC (garbage collection) can help reduce lag spikes with some mods. Only has effect after game is reloaded.";
		private const string CheckVersionTitle = "Check game version";
		private const string CheckVersionTooltip = "Whether or not to check the version of Outer Wilds when starting the game.";

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
			AddConfigInput(IncrementalGCTitle, _config.IncrementalGC, 4);
			AddConfigInput(CheckVersionTitle, _config.CheckVersion, 5);
			UpdateNavigation();
			SelectFirst();
		}

		public override void UpdateUIValues()
		{
			var debug = GetToggleInput(DebugModeTitle);
			debug.Value = _config.DebugMode;
			SetupInputTooltip(debug, DebugModeTooltip);

			var exe = GetToggleInput(ForceExeTitle);
			exe.Value = _config.ForceExe;
			SetupInputTooltip(exe, ForceExeTooltip);

			var gc = GetToggleInput(IncrementalGCTitle);
			gc.Value = _config.IncrementalGC;
			SetupInputTooltip(gc, IncrementalGCTooltip);

			var ver = GetToggleInput(IncrementalGCTitle);
			ver.Value = _config.CheckVersion;
			SetupInputTooltip(ver, CheckVersionTooltip);
		}

		protected override void OnSave()
		{
			_config.DebugMode = GetInputValue<bool>(DebugModeTitle);
			_config.ForceExe = GetInputValue<bool>(ForceExeTitle);
			_config.IncrementalGC = GetInputValue<bool>(IncrementalGCTitle);
			_config.CheckVersion = GetInputValue<bool>(CheckVersionTitle);
			JsonHelper.SaveJsonObject($"{_config.OWMLPath}{Constants.OwmlConfigFileName}", _config);
			Close();
		}

		protected override void OnReset()
		{
			_config.GamePath = _defaultConfig.GamePath;
			_config.DebugMode = _defaultConfig.DebugMode;
			_config.ForceExe = _defaultConfig.ForceExe;
			_config.IncrementalGC = _defaultConfig.IncrementalGC;
			_config.CheckVersion = _defaultConfig.CheckVersion;
			UpdateUIValues();
		}
	}
}
