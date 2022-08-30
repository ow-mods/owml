using OWML.Common;
using OWML.Common.Enums;
using OWML.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

			var gameDll = $"{_owmlConfig.ManagedPath}/Assembly-CSharp.dll";
			var assembly = Assembly.LoadFrom(gameDll);
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
