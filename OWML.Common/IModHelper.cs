namespace OWML.Common
{
    public interface IModHelper
    {
        IModLogger Logger { get; }
        IModConsole Console { get; }
        IModEvents Events { get; }
        IHarmonyHelper HarmonyHelper { get; }
    }
}
