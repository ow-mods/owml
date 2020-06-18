using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OWML.Common;
using OWML.Common.Menus;
using OWML.ModHelper.Input;

namespace OWML.ModHelper.Menus
{
    class ModInputCombinationMenu : ModPopupMenu
    {
        public List<IModInputCombinationElement> CombinationElements { get; private set; }

        public string Combination {
            get
            {
                string result = "";
                for (int i = 0; i < CombinationElements.Count; i++)
                {
                    result += CombinationElements[i].Title;
                    if (i<CombinationElements.Count-1)
                    {
                        result += "/";
                    }
                }
                return result;
            }
            set
            {
                CombinationElements.ForEach(x => x.DestroySelf());
                CombinationElements.Clear();
                foreach (var combination in value.Split('/'))
                {
                    CombinationElements.Add(_combinationElementTemplate.Copy(combination));
                }
            }
        }

        private IModInputCombinationElement _combinationElementTemplate;

        public ModInputCombinationMenu(IModInputCombinationElement combinationElementTemplate, IModConsole console):base(console)
        {
            CombinationElements = new List<IModInputCombinationElement>();
            _combinationElementTemplate = combinationElementTemplate;
        }

        public IModInputCombinationElement AddCombinationElement(IModInputCombinationElement element)
        {
            return AddCombinationElement(element, element.Index);
        }

        public IModInputCombinationElement AddCombinationElement(IModInputCombinationElement element, int index)
        {
            var transform = element.Toggle.transform;
            var scale = transform.localScale;
            transform.parent = layoutGroup.transform;
            element.Index = index;
            element.Initialize(this);
            CombinationElements.Add(element);
            element.Toggle.transform.localScale = scale;
            return element;
        }
    }
}
