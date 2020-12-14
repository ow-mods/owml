using UnityEngine;

namespace OWML.Common.Menus
{
	public interface IModInputBase
	{
		MonoBehaviour Element { get; }

		string Title { get; set; }

		int Index { get; set; }

		bool IsSelected { get; }

		void Show();

		void Hide();

		void Initialize(IModMenu menu);
	}
}