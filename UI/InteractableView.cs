using UnityEngine;
using UnityEngine.UI;

namespace Shibari.UI
{
    [RequireComponent(typeof(Selectable))]
    public class InteractableView : BindableView
    {
        private Selectable selectable;

        private static BindableValueRestraint[] bindableValueRestraints = new BindableValueRestraint[1] { new BindableValueRestraint(typeof(bool)) };
        public override BindableValueRestraint[] BindableValueRestraints { get { return bindableValueRestraints; } }

        protected override void Awake()
        {
            selectable = GetComponent<Selectable>();
            base.Awake();
        }

        protected override void OnValueChanged()
        {
            selectable.interactable = (bool) BindedValues[0].GetValue();
        }
    }
}