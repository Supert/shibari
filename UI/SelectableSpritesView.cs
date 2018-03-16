using UnityEngine;
using UnityEngine.UI;

namespace Shibari.UI
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(Selectable))]
    public class SelectableSpritesView : BindableView
    {
        private Selectable selectable;
        private Image image;

        private static BindableValueRestraint[] bindableValueRestraints = new BindableValueRestraint[1] { new BindableValueRestraint(typeof(SelectableSprites)) };
        public override BindableValueRestraint[] BindableValueRestraints { get { return bindableValueRestraints; } }

        protected override void Awake()
        {
            selectable = GetComponent<Selectable>();
            image = GetComponent<Image>();
            base.Awake();
        }

        protected override void OnValueChanged()
        {
            var sprites = BindedValues[0].GetValue() as SelectableSprites;
            image.sprite = sprites.Normal;
            var state = selectable.spriteState;
            state.disabledSprite = sprites.Disabled;
            state.highlightedSprite = sprites.Highlighted;
            state.pressedSprite = sprites.Pressed;
            selectable.spriteState = state;
        }
    }
}