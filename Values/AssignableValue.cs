using System.Collections.Generic;

namespace Shibari
{
    public sealed class AssignableValue<TValue> : BindableValue<TValue>
    {
        private TValue value;

        public override TValue Get()
        {
            return value;
        }

        public void Set(TValue value)
        {
            TValue oldValue = this.value;
            this.value = value;
            if (!EqualityComparer<TValue>.Default.Equals(oldValue, value))
                InvokeOnValueChanged();
        }
    }
}