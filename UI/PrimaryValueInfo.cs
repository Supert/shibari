using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Shibari
{
    public class AssignableValueInfo : BindableValueInfo
    {
        private MethodInfo setMethod;

        public AssignableValueInfo(PropertyInfo property, BindableData owner) : base(property, owner)
        {
            setMethod = BindableValue.GetType().GetMethod("Set", new Type[1] { ValueType });
        }

        public void SetValue(object o)
        {
            if (ValueType.IsValueType && o == null)
                throw new NullReferenceException($"Field type {ValueType} is value type, but argument is null");

            if (o == null)
                setMethod.Invoke(BindableValue, new object[1] { null });
            else
            {
                Type objectType = o?.GetType();

                if (!ValueType.IsAssignableFrom(objectType))
                {
                    if (typeof(Array).IsAssignableFrom(ValueType))
                    {
                        IList inputList = (IList)o;
                        Array array = (Array)Activator.CreateInstance(ValueType, new object[1] { inputList.Count });

                        inputList.CopyTo(array, 0);
                        setMethod.Invoke(BindableValue, new object[1] { array });
                    }
                    else if (ValueType.GetInterface(nameof(IDictionary)) != null)
                    {
                        IList inputList = (IList)o;
                        var dictionary = (IDictionary)Activator.CreateInstance(ValueType);
                        Type kvpType = typeof(KeyValuePair<,>).MakeGenericType(ValueType.GenericTypeArguments);
                        PropertyInfo kvpKey = kvpType.GetProperty("Key");
                        PropertyInfo kvpValue = kvpType.GetProperty("Value");
                        foreach (var kvp in inputList)
                            dictionary[kvpKey.GetValue(kvp)] = kvpValue.GetValue(kvp);
                        setMethod.Invoke(BindableValue, new object[1] { dictionary });
                    }
                    else if (ValueType.GetInterface(nameof(IList)) != null
                        && o is IList)
                    {
                        var inputList = (IList)o;
                        var list = (IList)Activator.CreateInstance(ValueType);
                        foreach (var element in inputList)
                            list.Add(element);
                        setMethod.Invoke(BindableValue, new object[1] { list });
                    }
                    else
                    {
                        throw new ArgumentException($"Field type {ValueType} is not assignable from argument type {o?.GetType()}");
                    }
                    return;
                }
                else
                {
                    setMethod.Invoke(BindableValue, new object[1] { o });
                }
            }
        }
    }
}