using System;

namespace Shibari
{
    public abstract class BindableValue<TValue> : IBindable
    {
        public event Action OnValueChanged;

        public void InvokeOnValueChanged()
        {
            OnValueChanged?.Invoke();
        }

        public abstract TValue Get();

        public static implicit operator TValue(BindableValue<TValue> bindableValue)
        {
            return bindableValue.Get();
        }

        public override string ToString()
        {
            return Get()?.ToString() ?? "{null}";
        }
    }
}