using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWML.Common
{
	public delegate void FloatOptionValueChangedEvent(float newValue);

	public interface IOWMLSliderElement : IOWMLMenuValueOption
	{
		public event FloatOptionValueChangedEvent OnValueChanged;
	}
}
