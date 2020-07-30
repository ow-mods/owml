using OWML.Common;
using System;
using System.Linq;
using System.Text;

namespace OWML.Patcher
{
    public class GameVersionReader
    {
        private readonly IModConsole _writer;
        private readonly BinaryPatcher _binaryPatcher;

        public GameVersionReader(IModConsole writer, BinaryPatcher binaryPatcher)
        {
            _writer = writer;
            _binaryPatcher = binaryPatcher;
        }
    }
}
