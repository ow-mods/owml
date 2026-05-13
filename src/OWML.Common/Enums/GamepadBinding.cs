namespace OWML.Common.Enums
{
	public enum GamepadBinding
	{
		None = -1,

		DPadUp = 0,
		DPadDown,
		DPadLeft,
		DPadRight,

		/// <summary>
		/// Xbox: Y, PS4: Triangle
		/// </summary>
		UpButton,
		/// <summary>
		/// Xbox: X, PS4: Square
		/// </summary>
		LeftButton,
		/// <summary>
		/// Xbox: A, PS4: Cross
		/// </summary>
		DownButton,
		/// <summary>
		/// Xbox: B, PS4: Circle
		/// </summary>
		RightButton,

		LeftStickClick,
		RightStickClick,

		LeftShoulder,
		RightShoulder,

		/// <summary>
		/// Xbox: Menu Button, PS4: Options Button
		/// </summary>
		Start,
		/// <summary>
		/// Xbox: View Button, PS4: Touchpad Press
		/// </summary>
		Select,

		LeftTrigger,
		RightTrigger,

		/// <summary>
		/// Xbox: Unused, PS4: Share Button
		/// </summary>
		/// <remarks>
		/// Even though the Share button is present on newer Xbox controllers, it doesn't have any functionality in Unity. It is only used as a capture/record button for Game Bar.
		/// </remarks>
		Share,
		/// <summary>
		/// Xbox: Unused, PS4: PlayStation Button
		/// </summary>
		/// <remarks>
		/// Even though the System button is present on all Xbox controllers, it doesn't have any functionality in Unity. It is only used to open Game Bar and/or Steam overlay.
		/// </remarks>
		SystemButton,

		LeftStickLeft,
		LeftStickRight,
		LeftStickUp,
		LeftStickDown,

		RightStickLeft,
		RightStickRight,
		RightStickUp,
		RightStickDown
	}
}
