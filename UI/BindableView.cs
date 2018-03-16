using UnityEngine;
using System;
using System.Linq;

namespace Shibari.UI
{
    public abstract class BindableView : MonoBehaviour
    {
        public abstract BindableValueRestraint[] BindableValueRestraints { get; }

        [SerializeField]
        private BindableValueSerializedInfo[] serializedInfos = new BindableValueSerializedInfo[0];

        public BindableValueSerializedInfo[] BindableValuesSerializedInfos { get { return serializedInfos; } set { serializedInfos = value; } }

        protected BindableValueInfo[] BindedValues { get; private set; }

        public void Initialize()
        {
            if (BindableValuesSerializedInfos == null || serializedInfos.Length != BindableValueRestraints.Length)
                serializedInfos = BindableValueRestraints.Select(bvt => new BindableValueSerializedInfo() { allowedValueType = bvt.Type }).ToArray();
            for (int i = 0; i < serializedInfos.Length; i++)
            {
                if (serializedInfos[i] == null)
                    serializedInfos[i] = new BindableValueSerializedInfo();
                if (serializedInfos[i].isSetterRequired != BindableValueRestraints[i].IsSetterRequired)
                    serializedInfos[i].isSetterRequired = BindableValueRestraints[i].IsSetterRequired;
                if (serializedInfos[i].allowedValueType == null || serializedInfos[i].allowedValueType != BindableValueRestraints[i].Type)
                    serializedInfos[i].allowedValueType = BindableValueRestraints[i].Type;
            }
        }

        protected virtual void Awake()
        {
            Initialize();
        }

        protected abstract void OnValueChanged();

        protected Delegate onValueChangedDelegate;

        protected virtual void Start()
        {
            onValueChangedDelegate = Delegate.CreateDelegate(typeof(Action), this, "OnValueChanged");
            BindedValues = new BindableValueInfo[serializedInfos.Length];

            for (int i = 0; i < serializedInfos.Length; i++)
            {
                BindedValues[i] = GetField(serializedInfos[i]);
                BindedValues[i].EventInfo.AddEventHandler(BindedValues[i].BindableValue, onValueChangedDelegate);
            }

            OnValueChanged();
        }

        protected BindableValueInfo GetField(BindableValueSerializedInfo ids)
        {
            return Model.RootNode.GetBindableValueByPath(ids.pathInModel.Split('/'));
        }

        protected void OnDestroy()
        {
            if (BindedValues != null)
            {
                foreach (var field in BindedValues)
                {
                    field.EventInfo.RemoveEventHandler(field.BindableValue, onValueChangedDelegate);
                }
            }
        }
    }
}