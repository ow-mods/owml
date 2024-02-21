using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWML.Common
{
	public delegate void TextEntryConfirmEvent();

	public interface IOWMLTextEntryElement : IOWMLMenuValueOption
	{
		public event TextEntryConfirmEvent OnConfirmEntry;

		public string GetInputText();
	}
}
