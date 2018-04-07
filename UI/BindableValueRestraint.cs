using System;

namespace Shibari.UI
{
    public class BindableValueRestraint
    {
        public Type Type { get; }
        public bool IsSetterRequired { get; }
        public string Label { get; }

        public BindableValueRestraint(Type type, bool isSetterRequired = false, string label = "")
        {
            Type = type;
            IsSetterRequired = isSetterRequired;
            Label = label;
        }
    }
}