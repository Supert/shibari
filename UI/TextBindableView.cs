using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

namespace Shibari.UI
{
    [RequireComponent(typeof(Text))]
    public class TextBindableView : BindableView
    {
        private BindableValueRestraint[] bindableValueTypes = new BindableValueRestraint[1] 
        {
            new BindableValueRestraint(typeof(object))
        };

        public override BindableValueRestraint[] BindableValueRestraints { get { return bindableValueTypes; } }

        protected Text text;

        protected override void Awake()
        {
            text = GetComponent<Text>();
            base.Awake();
        }

        protected override void OnValueChanged()
        {
            text.text = BindedValues[0].GetValue()?.ToString() ?? "{null}";
        }
    }
}