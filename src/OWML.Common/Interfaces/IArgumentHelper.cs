namespace OWML.Common.Interfaces
{
    public interface IArgumentHelper
    {
        string[] Arguments { get; }

        string GetArgument(string argument);

        bool HasArgument(string argument);

        void RemoveArgument(string argument);
    }
}