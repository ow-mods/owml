using System.Collections.Generic;
using System.Linq;
using OWML.Common;

namespace OWML.Launcher
{
    public class ArgumentHelper : IArgumentHelper
    {
        public string[] Arguments => _arguments.ToArray();

        private readonly List<string> _arguments;

        public ArgumentHelper(string[] args)
        {
            _arguments = args.ToList();
        }

        public string GetArgument(string name)
        {
            var index = _arguments.IndexOf($"-{name}");
            if (index == -1 || index >= _arguments.Count - 1)
            {
                return null;
            }
            return _arguments[index + 1];
        }

        public bool HasArgument(string name)
        {
            return GetArgument(name) != null;
        }

        public void RemoveArgument(string argument)
        {
            if (HasArgument(argument))
            {
                var index = _arguments.IndexOf($"-{argument}");
                _arguments.RemoveRange(index, 2);
            }
        }
    }
}
