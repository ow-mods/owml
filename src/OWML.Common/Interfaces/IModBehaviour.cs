using System.Collections.Generic;

namespace OWML.Common
{
	public interface IModBehaviour
	{
		IModHelper ModHelper { get; }

		object Api { get; }

		void Configure(IModConfig config);

		IList<IModBehaviour> GetDependants();

		IList<IModBehaviour> GetDependencies();

		object GetApi();

		/// <summary>
		/// Called when the title screen has loaded in.
		/// Put any code that edits the title menu here.
		/// </summary>
		void SetupTitleMenu(ITitleMenuManager titleManager);

		/// <summary>
		/// Called when leaving the title menu scene. Put any event unsubscriptions here.
		/// </summary>
		void CleanupTitleMenu();

		/// <summary>
		/// Called when the SolarSystem or EyeOfTheUniverse scene has loaded in.
		/// Put any code that edits the pause menu here.
		/// </summary>
		void SetupPauseMenu(IPauseMenuManager pauseManager);

		/// <summary>
		/// Called when leaving either game scene. Put any event unsubscriptions here.
		/// </summary>
		void CleanupPauseMenu();

		/// <summary>
		/// Called when the main menu or game has loaded in.
		/// Put any code that edits the options menu here.
		/// </summary>
		void SetupOptionsMenu(IOptionsMenuManager optionsManager);

		/// <summary>
		/// Called when leaving the title or either game scene. Put any event unsubscriptions here.
		/// </summary>
		void CleanupOptionsMenu();

		void Init(IModHelper helper);
	}
}
