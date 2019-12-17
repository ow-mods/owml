
namespace OWML.Common
{
    public class ModHelper : IModHelper
    {
        public IModLogger Logger { get; }
        public IModConsole Console { get; }
        public IModEvents Events { get; }
        public IHarmonyHelper HarmonyHelper { get; }

        public ModHelper(IModLogger logger, IModConsole console, IModEvents events, IHarmonyHelper harmonyHelper)
        {
            Logger = logger;
            Console = console;
            Events = events;
            HarmonyHelper = harmonyHelper;
        }

    }
}
