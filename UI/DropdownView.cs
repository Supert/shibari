using UnityEngine;
using UnityEngine.UI;

namespace Shibari.UI
{
    [RequireComponent(typeof(Dropdown))]
    public class DropdownView : BindableView
    {
        private Dropdown dropdown;

        private BindableValueRestraint[] bindableValueRestraints = new BindableValueRestraint[2]
        {
            new BindableValueRestraint(typeof(float), true),
            new BindableValueRestraint(typeof(string[]), false),
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
            dropdown = GetComponent<Dropdown>();
            dropdown.onValueChanged.AddListener((i) =>
            {
                if (i != (int)BindedValues[0].GetValue())
                    (BindedValues[0] as AssignableValueInfo).SetValue(i);
            });
            base.Awake();
        }

        protected override void OnValueChanged()
        {
            int value = (int)BindedValues[0].GetValue();
            if (value != dropdown.value)
                dropdown.value = (int)BindedValues[0].GetValue();
        }
    }
}