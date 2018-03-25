using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Shibari.Editor
{
    [CustomPropertyDrawer(typeof(UI.PathToHandler), true)]
    public class PathToHandlerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var value = property.FindPropertyRelative("value");
            value.stringValue = MultiLevelDropDownUtility.DrawControl(position, label, value.stringValue, BindableData.GetBindableHandlersPaths(Shibari.Model.RootNodeType, "").ToList());
        }
    }
}