using System;
using System.IO;
using OWML.Common;
using OWML.Tests.Setup;
using Xunit;
using Xunit.Abstractions;

namespace OWML.GameFinder.Tests
{
	public class SteamGameFinderTests : OWMLTests
	{
		public SteamGameFinderTests(ITestOutputHelper outputHelper)
			: base(outputHelper)
		{
		}

		[Fact]
		public void SteamGameFinder_FindGamePath()
		{
			var pathFinder = new SteamGameFinder(new OwmlConfig(), Console.Object);
			
			var gamePath = pathFinder.FindGamePath();

			Assert.Equal(new DirectoryInfo(SteamGamePath).FullName, new DirectoryInfo(gamePath).FullName, StringComparer.InvariantCultureIgnoreCase);
		}
	}
}
