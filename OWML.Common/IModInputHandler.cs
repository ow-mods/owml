namespace OWML.Common
{
    public interface IModInputHandler
    {
        RegistrationCode RegisterCombination(IModInputCombination combo);
        RegistrationCode UnregisterCombination(IModInputCombination combo);
        bool IsPressedExact(IModInputCombination combo);
        bool IsNewlyPressedExact(IModInputCombination combo, bool keep = false);
        bool WasTappedExact(IModInputCombination combo, bool keep = false);
        bool WasNewlyReleasedExact(IModInputCombination combo, bool keep = false);
        bool IsPressed(IModInputCombination combo);
        bool IsNewlyPressed(IModInputCombination combo, bool keep = false);
        bool WasTapped(IModInputCombination combo, bool keep = false);
        bool WasNewlyReleased(IModInputCombination combo, bool keep = false);
    }
}
