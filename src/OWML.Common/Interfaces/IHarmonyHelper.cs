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

		void EmptyMethod<T>(string methodName);
		void EmptyMethod(MethodBase methodInfo);

		void Transpile<T>(string methodName, Type patchType, string patchMethodName);
		void Transpile(MethodBase methodInfo, Type patchType, string patchMethodName);
	}
}
