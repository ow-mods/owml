using System;
using System.Linq;
using OWML.Common;

namespace OWML.ModHelper
{
    public class OutputWriter : IModConsole
    {
        public void WriteLine(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return;
            }
            if (line.ToLower().Contains("error") || line.ToLower().Contains("exception"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else if (line.ToLower().Contains("warning") || line.ToLower().Contains("disabled"))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            else if (line.ToLower().Contains("success"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            Console.WriteLine(line);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public void WriteLine(params object[] objects)
        {
            WriteLine(string.Join(" ", objects.Select(o => o.ToString()).ToArray()));
        }

    }
}
