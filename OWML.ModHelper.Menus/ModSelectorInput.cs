using OWML.Common.Menus;
using OWML.ModHelper.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace OWML.ModHelper.Menus
{
    public class ModSelectorInput : ModInput<string>, IModSelectorInput
    {
        public override bool IsSelected => _element?.GetValue<bool>("_amISelected") ?? false;

        private readonly OptionsSelectorElement _element;
        private int _count;
        private List<string> _options = new List<string>();

        public override string Value
        {
            get => _options[_element.GetCurrentIndex()];
            set => _element.Initialize(_options.IndexOf(value));
        }

        public int SelectedIndex
        {
            get => _element.GetCurrentIndex();
            set => _element.Initialize((value % _count + _count) % _count);
        }

        public ModSelectorInput(OptionsSelectorElement element, IModMenu menu) : base(element, menu)
        {
            _element = element;
            _count = element.GetValue<string[]>("_optionsList").Length;
            element.OnValueChanged += OnValueChanged;
        }

        private void OnValueChanged(int value)
        {
            if (_options.Count > value)
            {
                InvokeOnChange(_options[value]);
            }
        }

        public void Initialize(string option, string[] options)
        {
            _count = options.Length;
            _options = options.ToList();
            var index = _options.IndexOf(option);
            index = Math.Max(index, 0);
            _element.Initialize(index, options);
        }

        public IModSelectorInput Copy()
        {
            var copy = Object.Instantiate(_element);
            Object.Destroy(copy.GetComponentInChildren<LocalizedText>(true));
            return new ModSelectorInput(copy, Menu);
        }

        public IModSelectorInput Copy(string title)
        {
            var copy = Copy();
            copy.Title = title;
            return copy;
        }
    }
}
