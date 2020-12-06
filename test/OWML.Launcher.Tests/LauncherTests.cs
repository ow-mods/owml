using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Moq;
using OWML.Common.Enums;
using OWML.Common.Interfaces;
using Xunit;
using Xunit.Abstractions;

namespace OWML.Launcher.Tests
{
    public class LauncherTests // todo fix folders (src/test)
    {
        private readonly ITestOutputHelper _outputHelper;

        public LauncherTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact]
        public async Task Run_StartsGame()
        {
            var processHelper = new Mock<IProcessHelper>();
            var console = new Mock<IModConsole>();

            var args = new[] { "-consolePort", "1337" };

            var container = Program.CreateContainer(args);
            container.Add(processHelper.Object);
            container.Add(console.Object);

            console.Setup(s => s.WriteLine(It.IsAny<string>()))
                .Callback((string s) => _outputHelper.WriteLine(s));

            console.Setup(s => s.WriteLine(It.IsAny<string>(), It.IsAny<MessageType>()))
                .Callback((string s, MessageType type) => _outputHelper.WriteLine($"{type}: {s}"));

            var config = container.Resolve<IOwmlConfig>();
            config.OWMLPath = await Task.Run(SetupOWML);

            var app = container.Resolve<App>();
            app.Run();

            processHelper.Verify(s => s.Start($"{config.GamePath}/OuterWilds.exe", new string[] { }), Times.Once);
        }

        private string SetupOWML()
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
