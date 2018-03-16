using System;

namespace Shibari
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class SerializeValueAttribute : Attribute
    {

    }
}