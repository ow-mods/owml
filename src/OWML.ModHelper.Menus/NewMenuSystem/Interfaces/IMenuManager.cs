using OWML.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWML.ModHelper.Menus.NewMenuSystem.Interfaces
{
	public interface IMenuManager
	{
		public ITitleMenuManager TitleMenuManager { get; }

		public IPauseMenuManager PauseMenuManager { get; }

		public IOptionsMenuManager OptionsMenuManager { get; }

		public void CreateOWMLMenus(IList<IModBehaviour> modList);
	}
}
