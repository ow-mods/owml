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

		/// <summary>
		/// Keeps the Mod Options tab open, since it is ephemeral.
		/// Otherwise, a popup opening in the tab would close it.
		/// </summary>
		public void ForceModOptionsOpen(bool force);
	}
}
