using OWML.Common;
using OWML.Utils;

namespace OWML.ModHelper.Menus
{
	public class OwmlConfigMenu : ModConfigMenuBase
	{
		private const string BlockInputTitle = "Mod inputs can block game actions";

		private readonly IOwmlConfig _config;
		private readonly IOwmlConfig _defaultConfig;

		public OwmlConfigMenu(IModManifest manifest, IOwmlConfig config, IModStorage storage, IApplicationHelper appHelper)
			: base(manifest, storage)
		{
			_config = config;
			_defaultConfig = JsonHelper.LoadJsonObject<OwmlConfig>($"{appHelper.DataPath}/Managed/{Constants.OwmlDefaultConfigFileName}");
		}

		protected override void AddInputs()
		{
			AddConfigInput(BlockInputTitle, _config.BlockInput, 2);
			UpdateNavigation();
			SelectFirst();
		}

		protected override void UpdateUIValues()
		{
			GetToggleInput(BlockInputTitle).Value = _config.BlockInput;
		}

		protected override void OnSave()
		{
			_config.BlockInput = (bool)GetInputValue(BlockInputTitle);
			Storage.Save(_config, Constants.OwmlConfigFileName);
			Close();
		}

		protected override void OnReset()
		{
			_config.GamePath = _defaultConfig.GamePath;
			_config.BlockInput = _defaultConfig.BlockInput;
			UpdateUIValues();
		}
	}
}
