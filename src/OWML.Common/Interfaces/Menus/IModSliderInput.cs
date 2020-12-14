namespace OWML.Common.Menus
{
	public interface IModSliderInput : IModInput<float>
	{
		float Min { get; set; }

		float Max { get; set; }

		bool HasValueText { get; }

		IModSliderInput Copy();

		IModSliderInput Copy(string title);
	}
}
