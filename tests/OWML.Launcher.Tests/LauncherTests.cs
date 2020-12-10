using Moq;
using OWML.Common;
using OWML.Tests.Setup;
using Xunit;
using Xunit.Abstractions;

namespace OWML.Launcher.Tests
{
    public class LauncherTests : OWMLTests
    {
        public LauncherTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        [Fact]
        public void Run_StartsGame()
        {
            var processHelper = new Mock<IProcessHelper>();

            var container = Program.CreateContainer(new[] { "-consolePort", "1337" });
            container.Add(processHelper.Object);
            container.Add(Console.Object);
            container.Add(Logger.Object);

            var app = container.Resolve<App>();
            app.Run();

            processHelper.Verify(s => s.Start(Config.ExePath, new string[] { }), Times.Once);
        }

        [Fact]
        public void Run_OldVersion_StartsGame()
        {
            Config.GamePath = "C:/Program Files (x86)/Outer Wilds";

            var processHelper = new Mock<IProcessHelper>();

            var container = Program.CreateContainer(new[] { "-consolePort", "1337" });
            container.Add(processHelper.Object);
            container.Add(Console.Object);
            container.Add(Logger.Object);
            container.Add(Config);

            var app = container.Resolve<App>();
            app.Run();

            processHelper.Verify(s => s.Start(Config.ExePath, new string[] { }), Times.Once);
        }
    }
}
