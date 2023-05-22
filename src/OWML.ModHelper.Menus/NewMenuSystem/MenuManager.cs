using OWML.ModHelper.Menus.NewMenuSystem.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWML.ModHelper.Menus.NewMenuSystem
{
	public class MenuManager : IMenuManager
	{
		public ITitleMenuManager TitleMenuManager { get; private set; }
		public IPauseMenuManager PauseMenuManager { get; private set; }
		public IOptionsMenuManager OptionsMenuManager { get; private set; }

		public MenuManager()
		{
			TitleMenuManager = new TitleMenuManager();
			OptionsMenuManager = new OptionsMenuManager();
		}
	}
}