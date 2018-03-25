using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;
using UnityEditor;

namespace Shibari
{
    [InitializeOnLoad]
    public static class Model
    {
        public const string SETTINGS_PATH = "Assets/Shibari/Resources/ShibariSettings.prefab";
        public const string SERIALIZATION_TEMPLATES = "Assets/Shibari/Templates/";

        public static BindableData RootNode { get; private set; }
        public static Type RootNodeType { get; private set; }

        static Model()
        {
            DeserializeRootNodeType();
        }

        public static void InitializeSettingsPrefab()
        {
            var settingsPrefab = new GameObject();
            settingsPrefab.AddComponent<ShibariSettings>();
            string[] splittedPath = SETTINGS_PATH.Split('/');
            string builtFolders = splittedPath[0];
            for (int i = 1; i < splittedPath.Length - 1; i++)
            {
                if (!AssetDatabase.GetSubFolders(builtFolders).Contains(builtFolders + "/" + splittedPath[i]))
                    AssetDatabase.CreateFolder(builtFolders, splittedPath[i]);
                builtFolders += "/" + splittedPath[i];
            }

            PrefabUtility.CreatePrefab(SETTINGS_PATH, settingsPrefab);
        }

        public static void DeserializeRootNodeType()
        {
            ShibariSettings settings = Resources.Load<ShibariSettings>("ShibariSettings");
            if (settings == null)
                InitializeSettingsPrefab();
            settings = Resources.Load<ShibariSettings>("ShibariSettings");
            RootNodeType = GetBindableDataTypes().FirstOrDefault(t => t.FullName == settings.RootNodeType.value);
            if (RootNodeType == null)
            {
                Debug.LogError("Please, set root node type in Shibari/Settings menu.");
                return;
            }
            else if (RootNodeType.GetConstructor(new Type[0]) == null)
            {
                Debug.LogError($"Root node type {RootNodeType} should implement default public constructor");
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            RootNode = (BindableData)Activator.CreateInstance(RootNodeType);
            RootNode.Initialize();
        }

        public static IEnumerable<Type> GetBindableDataTypes()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            IEnumerable<Type> result = GetBindableDataTypesInAssembly(executingAssembly);

            result = result.Concat(executingAssembly.GetReferencedAssemblies().SelectMany(assembly => GetBindableDataTypesInAssembly(Assembly.Load(assembly))));
            return result;
        }

        public static string GenerateSerializationTemplate(Type t)
        {
            if (!typeof(BindableData).IsAssignableFrom(t))
                throw new ArgumentException("Type t should be child of BindableData", "t");

            return BindableDataJsonConverter.GenerateJsonTemplate(t);
        }

        public static string GenerateSerializationTemplate<T>() where T : BindableData
        {
            return GenerateSerializationTemplate(typeof(T));
        }

        private static IEnumerable<Type> GetBindableDataTypesInAssembly(Assembly assembly)
        {
            return assembly.GetTypes()
                .Where(t => !t.IsAbstract)
                .Where(t => typeof(BindableData).IsAssignableFrom(t));
        }
    }
}