using System.Collections.Generic;

namespace OWML.Common
{
	public interface IMenuManager
	{
		public ITitleMenuManager TitleMenuManager { get; }
		public IPauseMenuManager PauseMenuManager { get; }
		public IOptionsMenuManager OptionsMenuManager { get; }
		public IPopupMenuManager PopupMenuManager { get; }

		internal IList<IModBehaviour> ModList { get; set; }
	}
}
