using System;

namespace Shibari.UI
{
    public class BindableValueRestraint
    {
        public Type Type { get; }
        public bool IsSetterRequired { get; }

        public BindableValueRestraint(Type type, bool isSetterRequired = false)
        {
            Type = type;
            IsSetterRequired = isSetterRequired;
        }
    }
}