using System;

namespace Shibari
{
    public class BindableValueReflection
        {
            public string Name { get; }
            public Type Type { get; }
            public Type ValueType { get; }

            public BindableValueReflection(string name, Type type, Type valueType)
            {
                Name = name;
                Type = type;
                ValueType = valueType;
            }
        }
}