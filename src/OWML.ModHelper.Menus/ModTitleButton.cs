﻿using OWML.Common.Menus;
using UnityEngine;
using UnityEngine.UI;

namespace OWML.ModHelper.Menus
{
	public class ModTitleButton : ModButtonBase, IModButton
	{
		internal Text Text { get; }

		public virtual string Title
		{
			get => Text != null ? Text.text : "";
			set
			{
				GameObject.Destroy(Button.GetComponentInChildren<LocalizedText>());
				Text.text = value;
			}
		}

		public ModTitleButton(Button button, IModMenu menu)
			: base(button, menu)
		{
			Text = Button.GetComponentInChildren<Text>() ??
			       Button.gameObject.AddComponent<Text>();
		}

		public new IModButton Copy()
		{
			var button = (IModButton)base.Copy();
			button.Title = Title;
			return button;
		}

		public IModButton Copy(string title)
		{
			var copy = Copy();
			copy.Title = title;
			return copy;
		}

		public IModButton Copy(string title, int index)
		{
			var copy = Copy(title);
			copy.Index = index;
			return copy;
		}

		public IModButton Duplicate(string title)
		{
			var dupe = (IModButton)Duplicate();
			dupe.Title = title;
			return dupe;
		}

		public IModButton Duplicate(string title, int index)
		{
			var dupe = Duplicate(title);
			dupe.Index = index;
			return dupe;
		}

		public IModButton Replace(string title)
		{
			var replacement = (IModButton)Replace();
			replacement.Title = title;
			return replacement;
		}

		public IModButton Replace(string title, int index)
		{
			var replacement = Replace(title);
			replacement.Index = index;
			return replacement;
		}

	}
}
