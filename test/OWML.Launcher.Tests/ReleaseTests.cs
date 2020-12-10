using System.IO;
using System.Linq;
using OWML.Tests.Setup;
using Xunit;
using Xunit.Abstractions;

namespace OWML.Launcher.Tests
{
    public class ReleaseTests : OWMLTests
    {
        public ReleaseTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        [Fact]
        public void ReleaseContainsAllFiles()
        {
            AssertFolderContainsFiles(OwmlReleasePath, new[]
            {
                "OWML.Launcher.exe",
                "OWML.Manifest.json",
                "OWML.DefaultConfig.json",
                "OWML.Abstractions.dll",
                "OWML.Common.dll",
                "OWML.GameFinder.dll",
                "OWML.Logging.dll",
                "OWML.ModHelper.dll",
                "OWML.ModHelper.Assets.dll",
                "OWML.ModHelper.Events.dll",
                "OWML.ModHelper.Input.dll",
                "OWML.ModHelper.Interaction.dll",
                "OWML.ModHelper.Menus.dll",
                "OWML.ModLoader.dll",
                "OWML.Patcher.dll",
                "OWML.Utils.dll",
                "0Harmony.dll",
                "dnlib.dll",
                "dnpatch.dll",
                "Gameloop.Vdf.dll",
                "Microsoft.Practices.Unity.dll",
                "NAudio-Unity.dll",
                "Newtonsoft.Json.dll",
                "System.Runtime.Serialization.dll",
                "OWML.zip"
            });

            AssertFolderContainsFiles($"{OwmlReleasePath}/Mods/OWML.EnableDebugMode", new[]
            {
                "default-config.json",
                "manifest.json",
                "OWML.EnableDebugMode.dll"
            });

            AssertFolderContainsFiles($"{OwmlReleasePath}/Mods/OWML.LoadCustomAssets", new[]
            {
                "blaster-firing.wav",
                "config.json",
                "cubebundle",
                "cubebundle.manifest",
                "default-config.json",
                "duck.obj",
                "duck.png",
                "manifest.json",
                "OWML.LoadCustomAssets.dll",
                "savefile.json",
                "spiral-mountain.mp3"
            });
        }

        [Fact]
        public void ReleaseDoesNotContainGameFiles()
        {
            Assert.NotEmpty(Directory.GetFiles(OwmlReleasePath, "OWML*.dll", SearchOption.AllDirectories));
            Assert.Empty(Directory.GetFiles(OwmlReleasePath, "UnityEngine*.dll", SearchOption.AllDirectories));
            Assert.Empty(Directory.GetFiles(OwmlReleasePath, "Assembly-CSharp.dll", SearchOption.AllDirectories));
        }

        private void AssertFolderContainsFiles(string folder, string[] files) => 
            files.ToList().ForEach(file => AssertFileExists(folder, file));

        private void AssertFileExists(string folder, string file) => 
            Assert.True(File.Exists($"{folder}/{file}"), $"File doesn't exist: {folder}/{file}");
    }
}
