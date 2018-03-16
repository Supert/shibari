using UnityEngine;
using System;
using UnityEngine.UI;

namespace Shibari.UI
{
    [RequireComponent(typeof(Slider))]
    public class SliderView : BindableView
    {
        private Slider slider;

        private BindableValueRestraint[] bindableValueRestraints = new BindableValueRestraint[1]
        {
            new BindableValueRestraint(typeof(float), true)
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
            slider = GetComponent<Slider>();
            slider.onValueChanged.AddListener((f) =>
            {
                if (f != (float)BindedValues[0].GetValue())
                    (BindedValues[0] as AssignableValueInfo).SetValue(f);
            });
            base.Awake();
        }

        protected override void OnValueChanged()
        {
            float value = (float)BindedValues[0].GetValue();
            if (value != slider.value)
                slider.value = (float)BindedValues[0].GetValue();
        }
    }
}