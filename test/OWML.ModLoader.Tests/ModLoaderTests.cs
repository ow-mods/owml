using System;
using System.IO;
using Moq;
using OWML.Common.Enums;
using OWML.Common.Interfaces;
using Xunit;
using Xunit.Abstractions;

namespace OWML.ModLoader.Tests
{
    public class ModLoaderTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public ModLoaderTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact]
        public void LoadMods_LoadsMods()
        {
            var appHelper = new Mock<IApplicationHelper>();
            appHelper.Setup(s => s.DataPath)
                .Returns(() => "C:/Program Files/Epic Games/OuterWilds/OuterWilds_Data");
            appHelper.Setup(s => s.Version)
                .Returns(() => "1.3.3.7");

            var console = new Mock<IModConsole>();
            console.Setup(s => s.WriteLine(It.IsAny<string>()))
                .Callback((string s) => WriteLine(s));
            console.Setup(s => s.WriteLine(It.IsAny<string>(), It.IsAny<MessageType>()))
                .Callback((string s, MessageType type) => WriteLine($"{type}: {s}"));

            var logger = new Mock<IModLogger>();
            logger.Setup(s => s.Log(It.IsAny<string>()))
                .Callback((string s) => WriteLine(s));

            var modBehaviour = new Mock<IModBehaviour>();

            var goHelper = new Mock<IGameObjectHelper>();
            goHelper.Setup(s => s.CreateAndAdd<IModBehaviour>(It.IsAny<Type>(), It.IsAny<string>()))
                .Returns(() => modBehaviour.Object);
            goHelper.Setup(s => s.CreateAndAdd<IModUnityEvents, It.IsAnyType>(It.IsAny<string>()))
                .Returns(() => new Mock<IModUnityEvents>().Object);
            goHelper.Setup(s => s.CreateAndAdd<IBindingChangeListener, It.IsAnyType>(It.IsAny<string>()))
                .Returns(() => new Mock<IBindingChangeListener>().Object);

            var container = ModLoader.CreateContainer(appHelper.Object, goHelper.Object);
            container.Add(console.Object);
            container.Add(logger.Object);

            var config = container.Resolve<IOwmlConfig>();
            config.OWMLPath = GetOwmlPath();

            var owo = container.Resolve<Owo>();
            owo.LoadMods();

            modBehaviour.Verify(s => s.Init(It.IsAny<IModHelper>()), Times.Once());
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
