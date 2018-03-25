using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Shibari.Editor
{
    [CustomPropertyDrawer(typeof(PathToBindableData), true)]
    public class PathToBindableDataDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var value = property.FindPropertyRelative("value");
            value.stringValue = MultiLevelDropDownUtility.DrawControl(position, label, value.stringValue, Shibari.Model.GetBindableDataTypes().Select(t => t.FullName).ToList());
        }
    }
}