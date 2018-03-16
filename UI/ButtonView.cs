using UnityEngine;
using UnityEngine.UI;

namespace Shibari.UI
{
    [RequireComponent(typeof(Button))]
    public class ButtonView : BindableHandlerView
    {
        private Button button;

        protected virtual void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(Invoke);
        }
    }
}