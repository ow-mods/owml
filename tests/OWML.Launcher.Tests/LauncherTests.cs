using Moq;
using OWML.Common.Interfaces;
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
    }
}
