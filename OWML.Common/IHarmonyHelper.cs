using System;
using System.Reflection;

namespace OWML.Common
{
    /// <summary>
    /// Handler for patching methods.
    /// </summary>
    public interface IHarmonyHelper
    {
        /// <summary>Add prefix patch to the given method.</summary>
        /// <typeparam name="T">The type the method is in.</typeparam>
        /// <param name="methodName">The name of the method.</param>
        /// <param name="patchType">The type the patch is in.</param>
        /// <param name="patchMethodName">The name of the patch method.</param>
        void AddPrefix<T>(string methodName, Type patchType, string patchMethodName);

        /// <summary>Add prefix patch to the given method.</summary>
        /// <param name="methodInfo">The method to patch.</param>
        /// <param name="patchType">The type the patch is in.</param>
        /// <param name="patchMethodName">The name of the patch method.</param>
        void AddPrefix(MethodInfo methodInfo, Type patchType, string patchMethodName);

        /// <summary>Add postfix patch to the given method.</summary>
        /// <typeparam name="T">The type the method is in.</typeparam>
        /// <param name="methodName">The name of the method.</param>
        /// <param name="patchType">The type the patch is in.</param>
        /// <param name="patchMethodName">The name of the patch method.</param>
        void AddPostfix<T>(string methodName, Type patchType, string patchMethodName);

        /// <summary>Add postfix patch to the given method.</summary>
        /// <param name="methodInfo">The method to patch.</param>
        /// <param name="patchType">The type the patch is in.</param>
        /// <param name="patchMethodName">The name of the patch method.</param>
        void AddPostfix(MethodInfo methodInfo, Type patchType, string patchMethodName);

        /// <summary>Empty the given method.</summary>
        /// <typeparam name="T">The type the method is in.</typeparam>
        /// <param name="methodName">The name of the method.</param>
        void EmptyMethod<T>(string methodName);

        /// <summary>Empty the given method.</summary>
        /// <param name="methodInfo">The method to clear.</param>
        void EmptyMethod(MethodInfo methodInfo);

        /// <summary>Add transpile patch to the given method.</summary>
        /// <typeparam name="T">The type the method is in.</typeparam>
        /// <param name="methodName">The name of the method.</param>
        /// <param name="patchType">The type the patch is in.</param>
        /// <param name="patchMethodName">The name of the patch method.</param>
        void Transpile<T>(string methodName, Type patchType, string patchMethodName);

        /// <summary>Add transpile patch to the given method.</summary>
        /// <param name="methodInfo">The method to patch.</param>
        /// <param name="patchType">The type the patch is in.</param>
        /// <param name="patchMethodName">The name of the patch method.</param>
        void Transpile(MethodInfo methodInfo, Type patchType, string patchMethodName);
    }
}
