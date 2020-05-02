using System.Collections.Generic;
using OWML.Common;
using UnityEngine;

namespace OWML.ModHelper.Input
{
    public class ModInputCombination : IModInputCombination
    {
        public float LastPressedMoment => _lastPressedMoment;
        public float PressDuration => _lastPressedMoment - _firstPressedMoment;

        internal string Combo { get; private set; }

        private bool _isPressed = false, _isFirst = false;
        private float _firstPressedMoment = 0f, _lastPressedMoment = 0f;
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
                _lastPressedMoment = Time.realtimeSinceStartup;
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
            return Time.realtimeSinceStartup - _lastPressedMoment < relevancyKeep; 
        }
    }
}
