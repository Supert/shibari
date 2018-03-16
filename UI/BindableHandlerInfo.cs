using System.Reflection;
using UnityEngine;

namespace Shibari
{
    public class BindableHandlerInfo
    {
        public object obj;
        public MethodInfo method;

        public void Invoke(Component owner, params string[] args)
        {
            method.Invoke(obj, args);
        }
    }
}