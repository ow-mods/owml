using System;
using System.Linq;
using OWML.Common;

namespace OWML.ModHelper.Logging
{
    public class OutputWriter : IModConsole
    {
        public void WriteLine(string line)
        {
            Console.WriteLine(line);
        }

        public void WriteLine(params object[] objects)
        {
            WriteLine(string.Join(" ", objects.Select(o => o.ToString()).ToArray()));
        }
    }
}
