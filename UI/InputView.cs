﻿using UnityEngine;
using UnityEngine.UI;

namespace Shibari.UI
{
    [RequireComponent(typeof(InputField))]
    public class InputView : BindableView
    {
        private InputField input;

        private BindableValueRestraint[] bindableValueRestraints = new BindableValueRestraint[1]
        {
            new BindableValueRestraint(typeof(string), true)
        };

        public override BindableValueRestraint[] BindableValueRestraints
        {
            get
            {
                return bindableValueRestraints;
            }
        }

        protected override void Awake()
        {
            input = GetComponent<InputField>();
            input.onValueChanged.AddListener((s) =>
            {
                if (s != (string)BoundValues[0].GetValue())
                {
                    (BoundValues[0] as AssignableValueInfo).SetValue(s);
                }
            });
            base.Awake();
        }

        protected override void OnValueChanged()
        {
            string value = (string)BoundValues[0].GetValue();
            if (value != input.text)
                input.text = (string)BoundValues[0].GetValue();
        }
    }
}