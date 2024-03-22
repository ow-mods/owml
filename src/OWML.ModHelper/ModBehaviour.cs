using System.Collections.Generic;
using OWML.Common;
using UnityEngine;

namespace OWML.ModHelper
{
	public class ModBehaviour : MonoBehaviour, IModBehaviour
	{
		public IModHelper ModHelper { get; private set; }

		public object Api { get; private set; }

		public void Init(IModHelper modHelper)
		{
			ModHelper = modHelper;
			Configure(modHelper.Config);
			DontDestroyOnLoad(gameObject);
			Api = GetApi();
		}

		public virtual void Configure(IModConfig config)
		{
		}

		public virtual object GetApi() => null;

		public virtual void SetupTitleMenu(ITitleMenuManager titleManager) { }
		public virtual void CleanupTitleMenu() { }

		public virtual void SetupPauseMenu(IPauseMenuManager pauseManager) { }
		public virtual void CleanupPauseMenu() { }

		public virtual void SetupOptionsMenu(IOptionsMenuManager optionsManager) { }
		public virtual void CleanupOptionsMenu() { }

		public IList<IModBehaviour> GetDependants() =>
			ModHelper.Interaction.GetDependants(ModHelper.Manifest.UniqueName);

		public IList<IModBehaviour> GetDependencies() =>
			ModHelper.Interaction.GetDependencies(ModHelper.Manifest.UniqueName);
	}
}
