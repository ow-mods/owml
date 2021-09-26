using System;
using System.IO;
using Moq;
using OWML.Common;
using Xunit;
using Xunit.Abstractions;

namespace OWML.Tests.Setup
{
	public class OWMLTests
	{
		protected string EpicGamePath => "C:/Program Files/Epic Games/OuterWilds";
		
		protected string SteamGamePath => "C:/Program Files (x86)/Steam/steamapps/common/Outer Wilds";

		protected string OwmlSolutionPath => GetSolutionPath();

		protected string OwmlReleasePath => $"{OwmlSolutionPath}/src/OWML.Launcher/bin/Debug/net48/";

		protected Mock<IModConsole> Console { get; } = new();

		protected Mock<IModLogger> Logger { get; } = new();

		protected Mock<IApplicationHelper> AppHelper { get; } = new();

		protected Mock<IGameObjectHelper> GOHelper { get; } = new();

		protected IOwmlConfig Config { get; } = new OwmlConfig();
		
		private readonly ITestOutputHelper _outputHelper;

		public OWMLTests(ITestOutputHelper outputHelper)
		{
			_outputHelper = outputHelper;

			Config.OWMLPath = OwmlReleasePath;
			Config.GamePath = SteamGamePath;

			AppHelper.Setup(s => s.DataPath)
				.Returns(() => $"{SteamGamePath}/OuterWilds_Data");
			AppHelper.Setup(s => s.Version)
				.Returns(() => "1.3.3.7");

			Console.Setup(s => s.WriteLine(It.IsAny<string>()))
				.Callback((string s) => WriteLine(s));
			Console.Setup(s => s.WriteLine(It.IsAny<string>(), It.IsAny<MessageType>()))
				.Callback((string s, MessageType type) => WriteLine($"{type}: {s}"));

			Logger.Setup(s => s.Log(It.IsAny<string>()))
				.Callback((string s) => WriteLine(s));

			GOHelper.Setup(s => s.CreateAndAdd<IModBehaviour>(It.IsAny<Type>(), It.IsAny<string>()))
				.Returns(() =>
				{
					var mod = new Mock<IModBehaviour>();
					mod.Setup(s => s.Init(It.IsAny<IModHelper>()))
						.Callback((IModHelper modHelper) => mod.SetupGet(m => m.ModHelper)
							.Returns(modHelper));
					return mod.Object;
				});
			GOHelper.Setup(s => s.CreateAndAdd<IModUnityEvents, It.IsAnyType>(It.IsAny<string>()))
				.Returns(() => new Mock<IModUnityEvents>().Object);
		}

		private string GetSolutionPath() =>
			Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName;

		private void WriteLine(string s)
		{
			_outputHelper.WriteLine(s);
			Assert.DoesNotContain("Error", s, StringComparison.InvariantCultureIgnoreCase);
			Assert.DoesNotContain("Exception", s, StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
