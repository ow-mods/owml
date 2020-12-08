namespace OWML.Common.Interfaces.Menus
{
    public interface IModInputCombinationElement : IModToggleInput
    {
        IModLayoutManager Layout { get; }
        void Destroy();
        void DestroySelf();
        new IModInputCombinationElement Copy(string combination);
    }
}
