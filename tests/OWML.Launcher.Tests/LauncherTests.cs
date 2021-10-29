﻿using Moq;
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

			var app = container.Resolve<App>();
			app.Run();

			processHelper.Verify(s => s.Start($"{SteamGamePath}/OuterWilds.exe", new string[] { }), Times.Once);
		}
	}
}
