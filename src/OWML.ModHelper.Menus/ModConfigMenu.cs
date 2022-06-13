using OWML.Common;
using OWML.Common.Menus;
using System;
using System.Linq;

namespace OWML.ModHelper.Menus
{
	public class ModConfigMenu : ModConfigMenuBase, IModConfigMenu
	{
		private const string EnabledTitle = "Enabled";

		public IModData ModData { get; }

		public IModBehaviour Mod { get; }

		public ModConfigMenu(IModData modData, IModBehaviour mod, IModStorage storage, IModConsole console)
			: base(modData.Manifest, storage, console)
		{
			ModData = modData;
			Mod = mod;
		}

		protected override void AddInputs()
		{
			var index = 2;
			AddConfigInput(EnabledTitle, ModData.Config.Enabled, index++);
			foreach (var setting in ModData.Config.Settings)
			{
				AddConfigInput(setting.Key, setting.Value, index++);
			}
			UpdateNavigation();
			SelectFirst();
		}

		public override void UpdateUIValues()
		{
			GetToggleInput(EnabledTitle).Value = ModData.Config.Enabled;
			foreach (var setting in ModData.Config.Settings)
			{
				SetInputValue(setting.Key, setting.Value);
			}
		}

		protected override void OnSave()
		{
			ModData.Config.Enabled = GetInputValue<bool>(EnabledTitle);
			var keys = ModData.Config.Settings.Select(x => x.Key).ToList();
			foreach (var key in keys)
			{
				var value = GetInputValue(key);
				if (value != null)
				{
					ModData.Config.SetSettingsValue(key, value);
				}
			}
			ModData.Storage.Save(ModData.Config, Constants.ModConfigFileName);

			// Fixes "If one mod throws an exception in Configure all mod settings break" (#452)
			try
			{
				Mod?.Configure(ModData.Config);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Exception thrown when changing mod settings: {e.Message}, {e.StackTrace}", MessageType.Error);
			}

			Close();
		}

		protected override void OnReset()
		{
			ModData.ResetConfigToDefaults();
			UpdateUIValues();
		}
	}
}
