using System.Collections.Generic;

namespace OWML.Common.Menus
{
    public interface IModSelectorInput : IModInput<string>
    {
        int SelectedIndex { get; set; }
        void Initialize(string option, string[] options);
        void Initialize(string option, Dictionary<string, string> options);
        IModSelectorInput Copy();
        IModSelectorInput Copy(string title);
    }
}
