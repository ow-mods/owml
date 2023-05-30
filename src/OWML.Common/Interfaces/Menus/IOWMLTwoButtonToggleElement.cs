using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWML.Common
{
	public interface IOWMLTwoButtonToggleElement
	{
		public event BoolOptionValueChangedEvent OnValueChanged;
	}
}
