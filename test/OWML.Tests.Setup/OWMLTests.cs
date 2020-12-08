using System;
using System.IO;
using Moq;
using OWML.Common.Enums;
using OWML.Common.Interfaces;
using Xunit;
using Xunit.Abstractions;

namespace OWML.Tests.Setup
{
    public class OWMLTests
    {
        protected readonly Mock<IModConsole> Console = new Mock<IModConsole>();
        protected readonly Mock<IModLogger> Logger = new Mock<IModLogger>();
        protected readonly Mock<IApplicationHelper> AppHelper = new Mock<IApplicationHelper>();
        protected readonly Mock<IGameObjectHelper> GOHelper;
        protected readonly Mock<IModBehaviour> Mod = new Mock<IModBehaviour>();
        protected readonly Mock<IOwmlConfig> Config = new Mock<IOwmlConfig>();

        private readonly ITestOutputHelper _outputHelper;

        public OWMLTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;

            AppHelper.Setup(s => s.DataPath)
                .Returns(() => "C:/Program Files/Epic Games/OuterWilds/OuterWilds_Data");
            AppHelper.Setup(s => s.Version)
                .Returns(() => "1.3.3.7");

            Console.Setup(s => s.WriteLine(It.IsAny<string>()))
                .Callback((string s) => WriteLine(s));
            Console.Setup(s => s.WriteLine(It.IsAny<string>(), It.IsAny<MessageType>()))
                .Callback((string s, MessageType type) => WriteLine($"{type}: {s}"));

            Logger.Setup(s => s.Log(It.IsAny<string>()))
                .Callback((string s) => WriteLine(s));

            GOHelper = new Mock<IGameObjectHelper>();
            GOHelper.Setup(s => s.CreateAndAdd<IModBehaviour>(It.IsAny<Type>(), It.IsAny<string>()))
                .Returns(() => Mod.Object);
            GOHelper.Setup(s => s.CreateAndAdd<IModUnityEvents, It.IsAnyType>(It.IsAny<string>()))
                .Returns(() => new Mock<IModUnityEvents>().Object);
            GOHelper.Setup(s => s.CreateAndAdd<IBindingChangeListener, It.IsAnyType>(It.IsAny<string>()))
                .Returns(() => new Mock<IBindingChangeListener>().Object);

            var owmlPath = GetOwmlPath();
            //const string gamePath = "C:/Program Files/Epic Games/OuterWilds";

            Config.Setup(s => s.OWMLPath)
                .Returns(owmlPath);
            Config.Setup(s => s.ModsPath)
                .Returns(() => $"{owmlPath}Mods");
            Config.Setup(s => s.LogsPath)
                .Returns(() => $"{owmlPath}Logs");
            //Config.Setup(s => s.GamePath)
            //    .Returns(() => gamePath);
            //Config.Setup(s => s.DataPath)
            //    .Returns(() => $"{gamePath}/OuterWilds_Data");
            //Config.Setup(s => s.ManagedPath)
            //    .Returns(() => $"{gamePath}/OuterWilds_Data/Managed");
            //Config.Setup(s => s.PluginsPath)
            //    .Returns(() => $"{gamePath}/OuterWilds_Data/Plugins");
        }

        private string GetOwmlPath()
        {
            var currentFolder = Directory.GetCurrentDirectory();
            var owmlSolutionFolder = Directory.GetParent(currentFolder).Parent.Parent.Parent.FullName;
            return $"{owmlSolutionFolder}/Release/";
        }

        private void WriteLine(string s)
        {
            _outputHelper.WriteLine(s);
            Assert.DoesNotContain("Error", s, StringComparison.InvariantCultureIgnoreCase);
            Assert.DoesNotContain("Exception", s, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
