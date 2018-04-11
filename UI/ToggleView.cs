﻿using UnityEngine;
using UnityEngine.UI;

namespace Shibari.UI
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleView : BindableView
    {
        private Toggle toggle;

        private BindableValueRestraint[] bindableValueRestraints = new BindableValueRestraint[1]
        {
            new BindableValueRestraint(typeof(bool), true)
        };

        public override BindableValueRestraint[] BindableValueRestraints { get { return bindableValueRestraints; } }

        protected override void Awake()
        {
            toggle = GetComponent<Toggle>();
            toggle.onValueChanged.AddListener((b) =>
            {
                if (b != (bool)BoundValues[0].GetValue())
                    (BoundValues[0] as AssignableValueInfo).SetValue(b);
            });
            base.Awake();
        }

        protected override void OnValueChanged()
        {
            bool value = (bool)BoundValues[0].GetValue();
            if (value != toggle.isOn)
                toggle.isOn = (bool)BoundValues[0].GetValue();
        }
    }
}