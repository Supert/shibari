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
        public static BindableData RootNode { get; private set; }
        public static Type RootNodeType { get; private set; }

        static Model()
        {
            DeserializeRootNodeType();
        }

        public static void DeserializeRootNodeType()
        {
            ShibariSettings settings = Resources.Load<ShibariSettings>("ShibariSettings");
            RootNodeType = settings.RootNodeType.Type;
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