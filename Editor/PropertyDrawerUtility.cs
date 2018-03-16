using System;
using System.Linq;
using UnityEditor;
using System.Reflection;

namespace Shibari.Editor
{
    internal static class PropertyDrawerUtility
    {
        public static T GetActualObjectForSerializedProperty<T>(FieldInfo fieldInfo, SerializedProperty property) where T : class
        {
            var obj = fieldInfo.GetValue(property.serializedObject.targetObject);

            if (obj == null)
            {
                return null;
            }
            
            if (obj.GetType().IsArray)
            {
                var index = Convert.ToInt32(new string(property.propertyPath.SkipWhile(c => c != '[').TakeWhile(c => c != ']').Where(c => char.IsDigit(c)).ToArray()));
                return ((T[])obj)[index];
            }

            return obj as T;
        }
    }
}