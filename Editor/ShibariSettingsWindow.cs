using System.Linq;
using UnityEditor;

namespace Shibari.Editor
{
    public class ShibariSettingsWindow : EditorWindow
    {
        [MenuItem("Shibari/Settings")]
        private static void GetWindow()
        {
            ShibariSettingsWindow window = (ShibariSettingsWindow)GetWindow(typeof(ShibariSettingsWindow));
            window.Show();
        }

        void OnGUI()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<ShibariSettings>(Shibari.Model.SETTINGS_PATH);

            if (prefab == null)
                Shibari.Model.InitializeSettingsPrefab();
            prefab = AssetDatabase.LoadAssetAtPath<ShibariSettings>(Shibari.Model.SETTINGS_PATH);

            var editor = UnityEditor.Editor.CreateEditor(prefab);

            if (editor != null)
            {
                var so = editor.serializedObject;
                so.Update();
                editor.OnInspectorGUI();

                so.ApplyModifiedProperties();

                AssetDatabase.SaveAssets();
            }
        }
    }
}