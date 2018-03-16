using System;
using UnityEngine;
using UnityEngine.UI;

namespace Shibari.UI
{
    [RequireComponent(typeof(Image))]
    public class FillerBindableView : BindableView
    {
        private BindableValueRestraint[] bindableValueTypes = new BindableValueRestraint[1]
        {
            new BindableValueRestraint(typeof(Single)),
        };

        public override BindableValueRestraint[] BindableValueRestraints { get { return bindableValueTypes; } }

        private Image image;

        protected override void Start()
        {
            image = GetComponent<Image>();
            base.Start();

        }
        protected override void OnValueChanged()
        {
            image.fillAmount = Mathf.Clamp01((float)BindedValues[0].GetValue());
        }
    }
}