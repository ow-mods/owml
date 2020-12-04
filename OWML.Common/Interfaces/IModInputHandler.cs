using System.Collections.Generic;

namespace OWML.Common.Interfaces
{
    public interface IModInputHandler
    {
        IModInputTextures Textures { get; }

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
        List<string> GetWarningMessages(string combinations);
    }
}
