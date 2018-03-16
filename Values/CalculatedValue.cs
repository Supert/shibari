using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Shibari
{
    public class CalculatedValue<TValue> : BindableValue<TValue>
    {
        protected TValue Value { get; private set; }

        private Func<TValue> calculateValueFunction;

        public CalculatedValue(Func<TValue> calculateValueFunction, IEnumerable<IBindable> subscribeTo)
        {
            foreach (var bindable in subscribeTo)
            {
                if (bindable == null)
                    Debug.LogError($"Cannot subscribe to null IBindable");
                else
                    bindable.OnValueChanged += ValueChanged;
            }

            this.calculateValueFunction = calculateValueFunction;

            try
            {
                Value = calculateValueFunction.Invoke();
            }
            catch
            {

            }
        }

        public CalculatedValue(Func<TValue> calculateValueFunction, params IBindable[] subscribeTo) : this(calculateValueFunction, subscribeTo.AsEnumerable())
        {

        }

        protected virtual void ValueChanged()
        {
            if (calculateValueFunction == null)
            {
                Debug.LogError("calculateValueFunction is not assigned.");
                return;
            }
            Value = calculateValueFunction.Invoke();
            InvokeOnValueChanged();
        }

        public override TValue Get()
        {
            return Value;
        }
    }
}