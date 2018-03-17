﻿using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;

namespace Shibari.Editor
{
    [InitializeOnLoad]
    public class Model : UnityEditor.AssetModificationProcessor
    {
        static string[] OnWillSaveAssets(string[] paths)
        {
            if (paths.Any(p => p == Shibari.Model.SETTINGS_PATH))
                RefreshModel();
            return paths;
        }

        Model()
        {
            PrefabUtility.prefabInstanceUpdated += OnPrefabInstanceUpdate;
        }

        static void OnPrefabInstanceUpdate(GameObject instance)
        {
            GameObject prefab = PrefabUtility.GetPrefabParent(instance) as GameObject;

            ShibariSettings settings = prefab.GetComponent<ShibariSettings>();
            if (settings == null)
                return;

            string prefabPath = AssetDatabase.GetAssetPath(prefab);

            if (prefabPath != Shibari.Model.SETTINGS_PATH)
                Debug.Log($"Please, locate your shibari settings in \"{Shibari.Model.SETTINGS_PATH}\"");

            RefreshModel();
        }

        [DidReloadScripts]
        private static void OnDidReloadScripts()
        {
            RefreshModel();
        }

        public static void RefreshModel()
        {
            Shibari.Model.DeserializeRootNodeType();
        }

        public static void RefreshTemplates()
        {
            var types = Shibari.Model.GetBindableDataTypes().Where(t => BindableData.HasSerializeableValuesInChilds(t));
            foreach (var path in Directory.GetFiles(Shibari.Model.SERIALIZATION_TEMPLATES).Where(p => !types.Any(t => p == $"{Shibari.Model.SERIALIZATION_TEMPLATES}{t.FullName}.json")))
            {
                FileInfo file = new FileInfo($"{path}");
                file.Delete();
            }
            foreach (var type in types)
            {
                StreamWriter stream = File.CreateText($"{Shibari.Model.SERIALIZATION_TEMPLATES}{type.FullName}.json");
                stream.Flush();
                stream.Write(Shibari.Model.GenerateSerializationTemplate(type));
                stream.Close();
            }
            AssetDatabase.Refresh();
        }
    }
}
