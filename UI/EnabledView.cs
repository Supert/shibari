namespace Shibari.UI
{
    public class EnabledView : BindableView
    {
        private static BindableValueRestraint[] bindableValueRestraints = new BindableValueRestraint[1] { new BindableValueRestraint(typeof(bool)) };
        public override BindableValueRestraint[] BindableValueRestraints { get { return bindableValueRestraints; } }

        protected override void OnValueChanged()
        {
            gameObject.SetActive((bool)BindedValues[0].GetValue());
        }
    }
}