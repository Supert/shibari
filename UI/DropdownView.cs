using System.Linq;
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
            new BindableValueRestraint(typeof(int), true, "Value"),
            new BindableValueRestraint(typeof(string[]), false, "Options"),
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
            base.Awake();

            dropdown = GetComponent<Dropdown>();
            dropdown.onValueChanged.AddListener((i) =>
            {
                if (i != (int)BoundValues[0].GetValue())
                    (BoundValues[0] as AssignableValueInfo).SetValue(i);
            });
            
            dropdown.options = (BoundValues[1].GetValue() as string[]).Select(s => new Dropdown.OptionData(s)).ToList();
        }

        protected override void OnValueChanged()
        {
            int value = (int)BoundValues[0].GetValue();
            if (value != dropdown.value)
                dropdown.value = (int)BoundValues[0].GetValue();
        }
    }
}