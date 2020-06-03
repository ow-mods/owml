namespace OWML.Common
{
    /// <summary>
    /// Handler for writing to the console.
    /// </summary>
    public interface IModConsole
    {
        /// <summary>Write a string to the console.</summary>
        /// <param name="s">The string to be written.</param>
        void WriteLine(string s);

        /// <summary>Write an array of objects to the console.</summary>
        /// <param name="s">The array of objects to be written.</param>
        void WriteLine(params object[] s);
    }
}
