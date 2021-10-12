using System;
using System.Reflection;

namespace OWML.Common
{
	public interface IHarmonyHelper
	{
		void AddPrefix<T>(string methodName, Type patchType, string patchMethodName);

		void AddPrefix(MethodBase methodInfo, Type patchType, string patchMethodName);

		void AddPostfix<T>(string methodName, Type patchType, string patchMethodName);

		void AddPostfix(MethodBase methodInfo, Type patchType, string patchMethodName);

		void EmptyMethod<T>(string methodName, bool useTranspile = false);

		void EmptyMethod(MethodBase methodInfo, bool useTranspile = false);

		void Transpile<T>(string methodName, Type patchType, string patchMethodName);

		void Transpile(MethodBase methodInfo, Type patchType, string patchMethodName);

		void Unpatch<T>(string methodName, PatchType patchType = PatchType.All);
	}
}
