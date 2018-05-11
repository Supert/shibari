using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;

namespace Shibari
{
    public static class Model
    {
        public const string SETTINGS_PATH = "Assets/Shibari/Resources/ShibariSettings.prefab";
        public const string SERIALIZATION_TEMPLATES = "Assets/Shibari/Templates/";

        public static Node RootNode { get; private set; }
        public static Type RootNodeType { get; private set; }

        static Model()
        {
            DeserializeRootNodeType();
        }

        public static void DeserializeRootNodeType()
        {
            ShibariSettings settings = Resources.Load<ShibariSettings>("ShibariSettings");

            RootNodeType = GetNodeTypes().FirstOrDefault(t => t.FullName == settings.RootNodeType.value);
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
            if (RootNode == null)
            {
                RootNode = (Node)Activator.CreateInstance(RootNodeType);
                RootNode.Initialize();
            }
        }

        public static IEnumerable<Type> GetNodeTypes()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            IEnumerable<Type> result = GetNodeTypesInAssembly(executingAssembly);

            result = result.Concat(executingAssembly.GetReferencedAssemblies().SelectMany(assembly => GetNodeTypesInAssembly(Assembly.Load(assembly))));
            return result;
        }

        public static string GenerateSerializationTemplate(Type t)
        {
            if (!typeof(Node).IsAssignableFrom(t))
                throw new ArgumentException("Type t should be child of Shibari.Node", "t");

            return NodeJsonConverter.GenerateJsonTemplate(t);
        }

        public static string GenerateSerializationTemplate<T>() where T : Node
        {
            return GenerateSerializationTemplate(typeof(T));
        }

        private static IEnumerable<Type> GetNodeTypesInAssembly(Assembly assembly)
        {
            return assembly.GetTypes()
                .Where(t => !t.IsAbstract)
                .Where(t => typeof(Node).IsAssignableFrom(t));
        }
    }
}