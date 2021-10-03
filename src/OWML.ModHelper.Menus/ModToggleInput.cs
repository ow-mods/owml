using OWML.Common.Menus;
using OWML.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
	public class ModToggleInput : ModInput<bool>, IModToggleInput
	{
		public IModButton Button { get; }

		public ToggleElement Toggle { get; }

		public override bool IsSelected => Toggle.GetValue<bool>("_amISelected");

		public ModToggleInput(ToggleElement toggle, IModMenu menu)
			: base(toggle, menu)
		{
			Toggle = toggle;
			Button = new ModTitleButton(Toggle.GetComponent<Button>(), menu);
			Button.OnClick += () => InvokeOnChange(Value);
		}

		public override bool Value
		{
			get => Toggle.GetValueAsBool();
			set
			{
				Toggle.Initialize(value);
				InvokeOnChange(value);
			}
		}

		public virtual IModToggleInput Copy()
		{
			var copy = GameObject.Instantiate(Toggle);
			GameObject.Destroy(copy.GetComponentInChildren<LocalizedText>(true));
			return new ModToggleInput(copy, Menu);
		}

		public virtual IModToggleInput Copy(string title)
		{
			var copy = Copy();
			copy.Title = title;
			return copy;
		}
	}
}
