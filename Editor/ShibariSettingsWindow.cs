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
            var prefab = AssetDatabase.LoadAssetAtPath<ShibariSettings>(Model.SETTINGS_PATH);

            if (prefab == null)
            {
                var settingsPrefab = new UnityEngine.GameObject();
                prefab = settingsPrefab.AddComponent<ShibariSettings>();
                string[] splittedPath = Model.SETTINGS_PATH.Split('/');
                string builtFolders = splittedPath[0];
                for (int i = 1; i < splittedPath.Length - 1; i++)
                {
                    if (!AssetDatabase.GetSubFolders(builtFolders).Contains(splittedPath[i]))
                        AssetDatabase.CreateFolder(builtFolders, splittedPath[i]);
                    builtFolders += "/" + splittedPath[i];
                }

                PrefabUtility.CreatePrefab(Model.SETTINGS_PATH, settingsPrefab);
            }

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