﻿namespace OWML.Common
{
	public enum PatchType
	{
		All = 0,

		Prefix = 1,

		Postfix = 2,

		Transpiler = 3,

		Finalizer = 4,

		ILManipulator = 5,

		ReversePatch = 6
	}
}