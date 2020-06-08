using System;
using System.Reflection;

namespace OWML.Common
{
    /// <summary>
    /// Helper for patching methods.
    /// </summary>
    public interface IHarmonyHelper
    {
        /// <summary>Add prefix patch to the given method.</summary>
        /// <typeparam name="T">Type containing method being patched.</typeparam>
        /// <param name="methodName">Name of method being patched. Must be unique, with no overloads.</param>
        /// <param name="patchType">Type the patch method is in.</param>
        /// <param name="patchMethodName">Name of patch method. Must be a static method.</param>>
        void AddPrefix<T>(string methodName, Type patchType, string patchMethodName);

        /// <summary>Add prefix patch to the given method.</summary>
        /// <param name="methodInfo">The method being patched.</param>
        /// <param name="patchType">Type the patch method is in.</param>
        /// <param name="patchMethodName">Name of patch method. Must be a static method.</param>>
        void AddPrefix(MethodInfo methodInfo, Type patchType, string patchMethodName);

        /// <summary>Add postfix patch to the given method.</summary>
        /// <typeparam name="T">Type containing method being patched.</typeparam>
        /// <param name="methodName">Name of method being patched. Must be unique, with no overloads.</param>
        /// <param name="patchType">Type the patch method is in.</param>
        /// <param name="patchMethodName">Name of patch method. Must be a static method.</param>>
        void AddPostfix<T>(string methodName, Type patchType, string patchMethodName);

        /// <summary>Add postfix patch to the given method.</summary>
        /// <param name="methodInfo">The method being patched.</param>
        /// <param name="patchType">Type the patch method is in.</param>
        /// <param name="patchMethodName">Name of patch method. Must be a static method.</param>>
        void AddPostfix(MethodInfo methodInfo, Type patchType, string patchMethodName);

        /// <summary>Empty the given method.</summary>
        /// <typeparam name="T">The type the method is in.</typeparam>
        /// <param name="methodName">The name of the patch method. This method must exist in <typeparamref name="T"/> and must be static.</param>>
        void EmptyMethod<T>(string methodName);

        /// <summary>Empty the given method.</summary>
        /// <param name="methodInfo">The method to clear.</param>
        void EmptyMethod(MethodInfo methodInfo);

        /// <summary>Add transpile patch to the given method.</summary>
        /// <typeparam name="T">Type containing method being patched.</typeparam>
        /// <param name="methodName">Name of method being patched. Must be unique, with no overloads.</param>
        /// <param name="patchType">Type the patch method is in.</param>
        /// <param name="patchMethodName">Name of patch method. Must be a static method.</param>>
        void Transpile<T>(string methodName, Type patchType, string patchMethodName);

        /// <summary>Add transpile patch to the given method.</summary>
        /// <param name="methodInfo">The method being patched.</param>
        /// <param name="patchType">Type the patch method is in.</param>
        /// <param name="patchMethodName">Name of patch method. Must be a static method.</param>>
        void Transpile(MethodInfo methodInfo, Type patchType, string patchMethodName);
    }
}
