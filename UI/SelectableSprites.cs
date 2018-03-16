using UnityEngine;

namespace Shibari.UI
{
    public class SelectableSprites
    {
        public Sprite Normal { get; }
        public Sprite Highlighted { get; }
        public Sprite Pressed { get; }
        public Sprite Disabled { get; }

        public SelectableSprites(Sprite normal, Sprite highlighted, Sprite pressed, Sprite disabled)
        {
            Normal = normal;
            Highlighted = highlighted;
            Pressed = pressed;
            Disabled = disabled;
        }
    }
}