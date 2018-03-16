using UnityEngine;
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
                if (s != (string)BindedValues[0].GetValue())
                {
                    (BindedValues[0] as AssignableValueInfo).SetValue(s);
                }
            });
            base.Awake();
        }

        protected override void OnValueChanged()
        {
            string value = (string)BindedValues[0].GetValue();
            if (value != input.text)
                input.text = (string)BindedValues[0].GetValue();
        }
    }
}