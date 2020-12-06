using System;
using OWML.Common.Interfaces;

namespace OWML.Launcher
{
    public class ArgumentHelper : IArgumentHelper
    {
        private readonly string[] _arguments;

        public ArgumentHelper(string[] arguments)
        {
            _arguments = arguments;
        }

        public string GetArgument(string name)
        {
            var keyIndex = Array.IndexOf(_arguments, $"-{name}");
            if (keyIndex == -1 || keyIndex >= _arguments.Length - 1)
            {
                return null;
            }
            return _arguments[keyIndex + 1];
        }

        public bool HasArgument(string name)
        {
            return GetArgument(name) != null;
        }
    }
}
