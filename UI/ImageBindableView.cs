using Shibari.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace VillageKeeper.UI
{
    [RequireComponent(typeof(Image))]
    public class ImageBindableView : BindableView
    {
        private BindableValueRestraint[] bindableValueRestraints = new BindableValueRestraint[1] 
        {
           new BindableValueRestraint(typeof(Sprite))
        };

        public override BindableValueRestraint[] BindableValueRestraints { get { return bindableValueRestraints; } }

        private Image image;

        protected override void Awake()
        {
            image = GetComponent<Image>();
        }

        protected override void OnValueChanged()
        {
            var value = BindedValues[0].GetValue();
            image.sprite = (Sprite)value;
        }
    }
}