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
        private Dictionary<string, string> _converter;
        private Dictionary<string, string> _inverseConverter;

        public override string Value
        {
            get
            {
                var option = _options[_element.GetCurrentIndex()];
                return _inverseConverter == null ? option : _inverseConverter[option];
            }
            set => _element.Initialize(_options.IndexOf(_converter == null ? value : _converter[value]));
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

        public void Initialize(string option, Dictionary<string, string> options)
        {
            _count = options.Count;
            _options = options.Values.ToList();
            _converter = options;
            _inverseConverter = options.ToDictionary(pair => pair.Value, pair => pair.Key);
            var index = _options.IndexOf(options[option]);
            index = Math.Max(index, 0);
            _element.Initialize(index, _options.ToArray());
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
