using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
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
        public async Task Run_StartsGame()
        {
            var processHelper = new Mock<IProcessHelper>();

            var container = Program.CreateContainer(new[] { "-consolePort", "1337" });
            container.Add(processHelper.Object);
            container.Add(Console.Object);
            container.Add(Logger.Object);

            var config = container.Resolve<IOwmlConfig>();
            config.OWMLPath = await Task.Run(SetupOWML);

            var app = container.Resolve<App>();
            app.Run();

            processHelper.Verify(s => s.Start($"{config.GamePath}/OuterWilds.exe", new string[] { }), Times.Once);
        }

        private string SetupOWML() // todo separate test
        {
            var currentFolder = Directory.GetCurrentDirectory();
            var owmlSolutionFolder = Directory.GetParent(currentFolder).Parent.Parent.Parent.FullName;

            Process.Start(new ProcessStartInfo
            {
                FileName = $"{owmlSolutionFolder}/createrelease.bat",
                WorkingDirectory = owmlSolutionFolder,
                WindowStyle = ProcessWindowStyle.Hidden
            }).WaitForExit();

            return $"{owmlSolutionFolder}/Release/";
        }
    }
}
