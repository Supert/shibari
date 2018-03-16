using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Shibari.Editor
{
    [CustomPropertyDrawer(typeof(BindableValueSerializedInfo))]
    public class BindableValueSerializedInfoDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorStyles.popup.CalcHeight(GUIContent.none, 0);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var bindableIds = PropertyDrawerUtility.GetActualObjectForSerializedProperty<BindableValueSerializedInfo>(fieldInfo, property);
            var path = property.FindPropertyRelative("pathInModel"); 

            path.stringValue = MultiLevelDropDownUtility.DrawControl(
                position, 
                label, 
                property.FindPropertyRelative("pathInModel").stringValue, 
                BindableData.GetBindableValuesPaths(Shibari.Model.RootNodeType, "", false, true, bindableIds.allowedValueType).ToList()
            );

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}