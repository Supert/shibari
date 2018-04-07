using Shibari.UI;
using UnityEditor;
using UnityEngine;

namespace Shibari.Editor
{
    [CustomEditor(typeof(BindableView), true)]
    public class BindableViewEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            BindableView actualObject = serializedObject.targetObject as System.Object as BindableView;
            actualObject.Initialize();
            SerializedProperty tps = serializedObject.FindProperty("serializedInfos");
            EditorGUI.BeginChangeCheck();
            for (int i = 0; i < tps.arraySize; i++)
                EditorGUILayout.PropertyField(tps.GetArrayElementAtIndex(i), new GUIContent(actualObject.BindableValueRestraints[i].Label), true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}