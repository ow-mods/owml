namespace OWML.Common.Menus
{
    public interface IModInputCombinationElement : IModToggleInput
    {
        IModLayoutManager Layout { get; }
        void Destroy();
        void DestroySelf();
        new IModInputCombinationElement Copy(string combination);
        new string Title { get; set; }
    }
}
