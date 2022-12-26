using OWML.Common;
using OWML.Common.Enums;
using OWML.Common.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OWML.ModLoader
{
	public class GameVendorGetter : IGameVendorGetter
	{
		private readonly IModConsole _console;
		private readonly IOwmlConfig _owmlConfig;

		private GameVendor _gameVendor = GameVendor.None;

		public GameVendorGetter(IModConsole console, IOwmlConfig owmlConfig)
		{
			_console = console;
			_owmlConfig = owmlConfig;
		}

		public GameVendor GetGameVendor()
		{
			if (_gameVendor != GameVendor.None)
			{
				return _gameVendor;
			}

			var gameDll = Path.Combine(_owmlConfig.ManagedPath, "Assembly-CSharp.dll");

			Assembly assembly = null;
			try
			{
				assembly = Assembly.LoadFrom(gameDll);
			}
			catch (Exception ex)
			{
				_console.WriteLine($"Exception while trying to load game assembly to determine vendor: {ex}", MessageType.Error);
				return _gameVendor = GameVendor.None;
			}

			var types = assembly.GetTypes();

			if (types.Any(x => x.Name == "EpicEntitlementRetriever"))
			{
				_gameVendor = GameVendor.Epic;
			}
			else if (types.Any(x => x.Name == "SteamEntitlementRetriever"))
			{
				_gameVendor = GameVendor.Steam;
			}
			else
			{
				_gameVendor = GameVendor.Gamepass;
			}

			_console.WriteLine($"Detected vendor as {_gameVendor}.", MessageType.Debug);

			return _gameVendor;
		}
	}
}
