using HarmonyLib;
using OWML.Common;
using OWML.ModHelper.Menus.NewMenuSystem.Interfaces;
using OWML.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWML.ModHelper.Menus.NewMenuSystem
{
	public class MenuManager : IMenuManager
	{
		private readonly IModConsole _console;

		public ITitleMenuManager TitleMenuManager { get; private set; }
		public IPauseMenuManager PauseMenuManager { get; private set; }
		public IOptionsMenuManager OptionsMenuManager { get; private set; }

		public MenuManager(IModConsole console, IHarmonyHelper harmony, IModUnityEvents unityEvents)
		{
			_console = console;
			TitleMenuManager = new TitleMenuManager();
			OptionsMenuManager = new OptionsMenuManager(console, unityEvents);

			var harmonyInstance = harmony.GetValue<Harmony>("_harmony");
			harmonyInstance.PatchAll(typeof(Patches));
		}
	}
}