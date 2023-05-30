using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWML.Common
{
	public delegate void BoolOptionValueChangedEvent(bool newValue);

	public interface IOWMLToggleElement
	{
		public event BoolOptionValueChangedEvent OnValueChanged;
		
	}
}
