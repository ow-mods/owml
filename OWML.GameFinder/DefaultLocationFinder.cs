using System;
using System.Linq;
using OWML.Common;

namespace OWML.GameFinder
{
    public class DefaultLocationFinder : BaseFinder
    {
        private static readonly string[] DefaultLocations =
        {
            $"{AppDomain.CurrentDomain.BaseDirectory}..",
            "C:/Program Files (x86)/Outer Wilds",
            "D:/Program Files (x86)/Outer Wilds",
            "E:/Program Files (x86)/Outer Wilds",
            "F:/Program Files (x86)/Outer Wilds",
            "C:/Games/Outer Wilds",
            "D:/Games/Outer Wilds",
            "E:/Games/Outer Wilds",
            "F:/Games/Outer Wilds"
        };

        public DefaultLocationFinder(IOwmlConfig config, IModConsole writer) : base(config, writer)
        {
        }

        public override string FindGamePath()
        {
            var gamePath = DefaultLocations.FirstOrDefault(IsValidGamePath);
            if (!string.IsNullOrEmpty(gamePath))
            {
                return gamePath;
            }
            Writer.WriteLine("Game not found in default locations.");
            return null;
        }
    }
}
