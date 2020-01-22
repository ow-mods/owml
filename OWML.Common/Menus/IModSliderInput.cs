namespace OWML.Common.Menus
{
    public interface IModSliderInput : IModInput<float>
    {
        IModSliderInput Copy();
        IModSliderInput Copy(string title);
    }
}
