using System;

namespace Shibari
{
    [Serializable]
    public class BindableValueSerializedInfo
    {
        public Type allowedValueType;
        public bool isSetterRequired;
        public string pathInModel;
    }
}