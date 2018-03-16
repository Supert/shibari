using System.Reflection;
using System;

namespace Shibari
{
    public class BindableValueInfo
    {
        public object BindableValue { get; private set; }
        public EventInfo EventInfo { get; private set; }
        public PropertyInfo Property { get; private set; }
        public Type ValueType { get; private set; }

        private MethodInfo getMethod;
        private MethodInfo invokeOnValueChangedMethod;

        public BindableValueInfo(PropertyInfo property, BindableData owner)
        {
            Property = property;
            BindableValue = property.GetValue(owner);

            EventInfo = property.PropertyType.GetEvent("OnValueChanged");
            ValueType = property.PropertyType.GetGenericArguments()[0];
            getMethod = BindableValue.GetType().GetMethod("Get", new Type[0]);
            invokeOnValueChangedMethod = BindableValue.GetType().GetMethod("InvokeOnValueChanged", new Type[0]);
        }

        public object GetValue()
        {
            return getMethod.Invoke(BindableValue, new object[0]);
        }

        public void InvokeOnValueChanged()
        {
            invokeOnValueChangedMethod.Invoke(BindableValue, new object[0]);
        }
    }
}