#pragma warning disable 1591

namespace OWML.Common.Menus
{
    public interface IModSliderInput : IModInput<float>
    {
        float Min { get; set; }
        float Max { get; set; }

        IModSliderInput Copy();
        IModSliderInput Copy(string title);
    }
}
