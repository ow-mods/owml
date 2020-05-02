namespace OWML.Common
{
    public interface IModInputCombination
    {
        float LastPressedMoment { get; }
        float PressDuration { get; }
        bool IsFirst(bool keep = false);
    }
}
