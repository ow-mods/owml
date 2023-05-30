using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWML.Common
{
	public delegate void OptionValueChangedEvent(int newIndex, string newSelection);

	public interface IOWMLOptionsSelectorElement
	{
		public event OptionValueChangedEvent OnValueChanged;
	}
}
