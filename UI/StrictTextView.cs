using UnityEngine;
using UnityEngine.UI;

namespace Shibari.UI
{
    [RequireComponent(typeof(Text))]
    public class StrictTextView : TextView
    {
        private BindableValueRestraint[] bindableValueTypes = new BindableValueRestraint[1]
        {
            new BindableValueRestraint(typeof(string))
        };

        public override BindableValueRestraint[] BindableValueRestraints { get { return bindableValueTypes; } }
    }
}