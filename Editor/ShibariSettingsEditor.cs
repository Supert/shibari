using UnityEngine;
using UnityEditor;

namespace Shibari.Editor
{
    [CustomEditor(typeof(ShibariSettings))]
    public class ShibariSettingsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var prefab = PrefabUtility.GetPrefabObject(target);
            if (PrefabUtility.GetPrefabParent(target) == null && prefab != null && AssetDatabase.GetAssetPath(prefab) == Shibari.Model.SETTINGS_PATH)
            {
                serializedObject.Update();

                float w = Screen.width;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("RootNodeType"));

                serializedObject.ApplyModifiedProperties();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Refresh Templates"))
                {
                    Model.RefreshModel();
                    Model.RefreshTemplates();
                }

                if (GUILayout.Button("Apply"))
                {
                    Model.RefreshModel();
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.LabelField($"Please, make sure that you edit Shibari Settings via Shibari/Settings menu or directly at {Shibari.Model.SETTINGS_PATH}.");
            }
        }
    }
}