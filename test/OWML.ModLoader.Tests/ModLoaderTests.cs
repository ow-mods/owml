using System;
using System.IO;
using Moq;
using OWML.Common.Enums;
using OWML.Common.Interfaces;
using OWML.Common.Interfaces.Menus;
using Xunit;

namespace OWML.ModLoader.Tests
{
    public class ModLoaderTests
    {
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
                .Callback((string s) => Console.WriteLine(s));
            console.Setup(s => s.WriteLine(It.IsAny<string>(), It.IsAny<MessageType>()))
                .Callback((string s, MessageType type) => Console.WriteLine($"{type}: {s}"));

            var logger = new Mock<IModLogger>();
            logger.Setup(s => s.Log(It.IsAny<string>()))
                .Callback((string s) => Console.WriteLine(s));

            var modBehaviour = new Mock<IModBehaviour>();

            var goHelper = new Mock<IGameObjectHelper>();
            goHelper.Setup(s => s.CreateAndAdd(It.IsAny<Type>(), It.IsAny<string>()))
                .Returns(() => modBehaviour.Object);

            var container = ModLoader.CreateContainer(appHelper.Object);
            container.Add(console.Object);
            container.Add(logger.Object);
            container.Add(goHelper.Object);
            container.Add(new Mock<IModUnityEvents>().Object);
            container.Add(new Mock<IModMenus>().Object);
            container.Add(new Mock<IModInputHandler>().Object);

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
    }
}
