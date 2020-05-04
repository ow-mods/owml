using System.Collections.Generic;
using OWML.Common;
using UnityEngine;

namespace OWML.ModHelper.Input
{
    public class ModInputCombination : IModInputCombination
    {
        public float LastPressedMoment { get; private set; }
        public float PressDuration => LastPressedMoment - _firstPressedMoment;

        internal string Combo { get; }

        private bool _isPressed;
        private bool _isFirst;
        private float _firstPressedMoment;
        private List<KeyCode> _singles = new List<KeyCode>();

        public ModInputCombination(string combination)
        {
            Combo = combination;
        }

        public bool IsFirst(bool keep = false)
        {
            if (_isFirst)
            {
                _isFirst = keep;
                return true;
            }
            return false;
        }

        internal void SetPressed(bool isPressed = true)
        {
            if (isPressed)
            {
                if (!_isPressed)
                {
                    _isFirst = true;
                    _firstPressedMoment = Time.realtimeSinceStartup;
                }
                LastPressedMoment = Time.realtimeSinceStartup;
            }
            else
            {
                _isFirst = true;
            }
            _isPressed = isPressed;
        }

        internal void AddSingle(KeyCode button)
        {
            _singles.Add(button);
        }

        internal List<KeyCode> GetSingles()
        {
            return _singles;
        }

        internal void ClearSingles()
        {
            _singles.Clear();
        }

        internal bool IsRelevant(float relevancyKeep)
        {
            return Time.realtimeSinceStartup - LastPressedMoment < relevancyKeep;
        }
    }
}
