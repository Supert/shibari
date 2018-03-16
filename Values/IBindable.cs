using System;

namespace Shibari
{
    public interface IBindable
    {
        event Action OnValueChanged;
    }
}