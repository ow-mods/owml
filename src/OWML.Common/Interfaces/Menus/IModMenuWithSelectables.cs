using System;

namespace OWML.Common.Menus
{
	public interface IModMenuWithSelectables : IModPopupMenu
	{
		event Action OnCancel;
	}
}
