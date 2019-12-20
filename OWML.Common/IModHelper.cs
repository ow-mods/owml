namespace OWML.Common
{
    public interface IModHelper
    {
        IModConfig Config { get; }
        IModLogger Logger { get; }
        IModConsole Console { get; }
        IModEvents Events { get; }
        IHarmonyHelper HarmonyHelper { get; }
    }
}
