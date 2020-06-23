using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace OWML.Common
{
    public interface IModInputHandler
    {
        IModInputCombination RegisterCombination(IModBehaviour mod, string name, string combination);
        void UnregisterCombination(IModInputCombination combo);
        bool IsPressedExact(IModInputCombination combo);
        bool IsNewlyPressedExact(IModInputCombination combo);
        bool WasTappedExact(IModInputCombination combo);
        bool WasNewlyReleasedExact(IModInputCombination combo);
        bool IsPressed(IModInputCombination combo);
        bool IsNewlyPressed(IModInputCombination combo);
        bool WasTapped(IModInputCombination combo);
        bool WasNewlyReleased(IModInputCombination combo);
        List<string> GetCollisions(string combinations);
    }
}
