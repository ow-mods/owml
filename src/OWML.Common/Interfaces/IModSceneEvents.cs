using System;

namespace OWML.Common
{
	public interface IModSceneEvents
	{
		[Obsolete("Use HarmonyHelper instead.")]
		event Action<OWScene, OWScene> OnStartSceneChange;

		[Obsolete("Use HarmonyHelper instead.")]
		event Action<OWScene, OWScene> OnCompleteSceneChange;
	}
}
