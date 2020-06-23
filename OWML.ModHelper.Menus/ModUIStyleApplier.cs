using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using OWML.Common.Menus;
using System.Collections.Generic;
using OWML.ModHelper.Events;

namespace OWML.ModHelper.Menus
{
    public class ModUIStyleApplier : UIStyleApplier
    {
        public ModUIStyleApplier():base()
        {
            ClearAllArrays();
        }

        public void Initialize(bool isButton = false, bool isSecondary = false)
        {
            if (isButton)
            {
                SetAsButton();
            }    
            else
            {
                _buttonItem = isButton;
                _secondaryMenuItem = isSecondary;
            }
        }

        public void Initialize(UIStyleApplier oldStyleApplier)
        {
            var fields = typeof(UIStyleApplier).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            Array.ForEach<FieldInfo>(fields, field => field.SetValue(this, field.GetValue(oldStyleApplier)));
        }

        public void ClearAllArrays()
        {
            _textItems = new Text[0];
            _foregroundGraphics = new Graphic[0];
            _backgroundGraphics = new Graphic[0];
            _onOffGraphics = new Graphic[0];
            _onOffGraphicList = new OnOffGraphic[0];
        }

        public void SetTexts(Text[] texts)
        {
            _textItems = texts;
        }

        public void SetForeground(Graphic[] foreground)
        {
            _foregroundGraphics = foreground;
        }

        public void SetBackround(Graphic[] background)
        {
            _backgroundGraphics = background;
        }

        public void SetOnOffGraphics(Graphic[] OnOffs)
        {
            _onOffGraphics = OnOffs;
        }

        public void SetOnOffExtended(OnOffGraphic[] OnOffs)
        {
            _onOffGraphicList = OnOffs;
        }

        public void SetAsButton()
        {
            _buttonItem = true;
            _secondaryMenuItem = false;
        }

        public void SetAsSecondary()
        {
            _buttonItem = false;
            _secondaryMenuItem = true;
        }

        public void SetAsDefault()
        {
            _buttonItem = false;
            _secondaryMenuItem = false;
        }

        public static ModUIStyleApplier ReplaceStyleApplier(GameObject obj)
        {
            var oldUIStyleApplier = obj.GetComponent<UIStyleApplier>();
            var newUIStyleApplier = obj.AddComponent<ModUIStyleApplier>();
            if (oldUIStyleApplier != null)
            {
                newUIStyleApplier.Initialize(oldUIStyleApplier);
                GameObject.Destroy(oldUIStyleApplier);
            }
            return newUIStyleApplier;
        }
    }
}
