using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Shibari
{
    public abstract class BindableData
    {
        public ReadOnlyDictionary<string, BindableValueInfo> Values { get; private set; }
        public ReadOnlyDictionary<string, AssignableValueInfo> AssignableValues { get; private set; }
        public ReadOnlyDictionary<string, MethodInfo> BindableHandlers { get; private set; }

        public ReadOnlyDictionary<string, BindableData> Childs { get; private set; }

        private static readonly BindableDataJsonConverter converter = new BindableDataJsonConverter();

        #region public static methods
        public static IEnumerable<string> GetBindableHandlersPaths(Type type, string prefix)
        {
            IEnumerable<string> result = GetBindableDatas(type)
                .SelectMany(property => GetBindableHandlersPaths(property.PropertyType, prefix + property.Name + "/"));

            return result.Concat(GetBindableHandlers(type).Select(m => $"{prefix}{GetNameAndParamsFromMethodInfo(m)}"));
        }

        public static IEnumerable<string> GetBindableValuesPaths(Type type, string prefix, bool isSetterRequired, bool isVisibleInEditorRequired, Type valueType = null)
        {
            IEnumerable<string> result = GetBindableDatas(type)
                .SelectMany(property => GetBindableValuesPaths(property.PropertyType, prefix + property.Name + "/", isSetterRequired, isVisibleInEditorRequired, valueType));
            if (isSetterRequired)
            {
                return result
                        .Concat(GetAssignableValues(type)
                            .Where(property =>
                                IsEditorVisibilityAcceptable(property, isVisibleInEditorRequired)
                                && (valueType == null || valueType.IsAssignableFrom(GetBindableValueValueType(property.PropertyType))))
                            .Select(property => prefix + property.Name)).ToList();
            }
            else
            {
                return result
                    .Concat(GetBindableValues(type)
                        .Where(property =>
                            IsEditorVisibilityAcceptable(property, isVisibleInEditorRequired)
                            && (valueType == null || valueType.IsAssignableFrom(GetBindableValueValueType(property.PropertyType))))
                        .Select(property => prefix + property.Name)).ToList();
            }
        }

        public BindableValueInfo GetBindableValueByPath(string[] pathInModel)
        {
            if (pathInModel.Length > 1)
                return Childs[pathInModel[0]].GetBindableValueByPath(pathInModel.Skip(1).ToArray());
            else
                return Values[pathInModel[0]];
        }

        public void InvokeHandlerByPath(string[] pathInModel, UI.BindableHandlerView view, string data)
        {
            if (pathInModel.Length > 1)
            {
                Childs[pathInModel[0]].InvokeHandlerByPath(pathInModel.Skip(1).ToArray(), view, data);
            }
            else
            {
                MethodInfo method = BindableHandlers[pathInModel[0]];
                ParameterInfo[] parameters = method.GetParameters();
                if (parameters.Length == 0)
                    method.Invoke(this, new object[0]);
                else if (parameters.Length == 1 && parameters[0].ParameterType == typeof(UI.BindableHandlerView))
                    method.Invoke(this, new object[1] { view });
                else if (parameters.Length == 1 && parameters[0].ParameterType == typeof(string))
                    method.Invoke(this, new object[1] { data });
                else
                    method.Invoke(this, new object[2] { view, data });
            }
        }

        public static IEnumerable<PropertyInfo> GetBindableDatas(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(p => !p.GetMethod.IsPrivate)
                .Where(p => IsBindableData(p.PropertyType));
        }

        public static IEnumerable<PropertyInfo> GetBindableValues(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(p => !p.GetMethod.IsPrivate)
                    .Where(p => IsBindableValue(p.PropertyType));
        }

        public static IEnumerable<MethodInfo> GetBindableHandlers(Type type)
        {
            return type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => !m.IsPrivate)
                .Where(m => m.GetCustomAttribute(typeof(ShowInEditorAttribute)) != null)
                .Where(m => IsHandlerSignatureCorrect(m));
        }

        public static IEnumerable<PropertyInfo> GetAssignableValues(Type type)
        {
            return GetBindableValues(type).Where(p => CheckTypeTreeByPredicate(type, (t) => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(AssignableValue<>)));
        }

        public static IEnumerable<PropertyInfo> GetSerializableValues(Type type)
        {
            return GetBindableValues(type).Where(p => IsSerializableValue(p));
        }

        public static bool IsHandlerSignatureCorrect(MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();
            if (parameters.Length == 0)
                return true;
            if (parameters.Length == 1 && (parameters[0].ParameterType == typeof(UI.BindableHandlerView) || parameters[0].ParameterType == typeof(string)))
                return true;
            if (parameters.Length == 2 && parameters[0].ParameterType == typeof(UI.BindableHandlerView) && parameters[1].ParameterType == typeof(string))
                return true;
            return false;
        }

        public static bool IsBindableValue(Type propertyType)
        {
            return CheckTypeTreeByPredicate(propertyType, (t) => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(BindableValue<>));
        }

        public static bool IsAssignableValue(Type propertyType)
        {
            return CheckTypeTreeByPredicate(propertyType, (t) => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(AssignableValue<>));
        }

        public static bool IsCalculatedValue(Type propertyType)
        {
            return CheckTypeTreeByPredicate(propertyType, (t) => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(CalculatedValue<>));
        }

        public static bool IsSerializableValue(Type modelType, string propertyName)
        {
            var property = modelType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return IsSerializableValue(property);
        }

        public static bool IsSerializableValue(PropertyInfo property)
        {
            return property.GetCustomAttribute<SerializeValueAttribute>() != null
                && CheckTypeTreeByPredicate(property.PropertyType, (t) => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(AssignableValue<>));
        }

        public static bool IsBindableData(Type type)
        {
            var result = typeof(BindableData).IsAssignableFrom(type);
            return result;
        }

        public static BindableData GetDeserializedData(string serialized, Type type)
        {
            if (!typeof(BindableData).IsAssignableFrom(type))
            {
                throw new ArgumentException($"Type {typeof(BindableData)} is not assignable from type {type}");
            }
            return (BindableData)JsonConvert.DeserializeObject(serialized, type, converter);
        }

        public static Type GetBindableValueValueType(Type propertyType)
        {
            Type t = propertyType;
            while (!(t.IsGenericType && t.GetGenericTypeDefinition() == typeof(BindableValue<>)))
            {
                t = t.BaseType;

                if (t == typeof(object))
                    throw new ArgumentException("Property type is not BindableValue<>.", "propertyType");
            }

            return t.GetGenericArguments()[0];
        }

        public static T GetDeserializedData<T>(string serialized) where T : BindableData
        {
            return (T)GetDeserializedData(serialized, typeof(T));
        }


        public static bool HasSerializeableValuesInChilds(Type t)
        {
            return GetSerializableValues(t).Any() || GetBindableDatas(t).Any(b => HasSerializeableValuesInChilds(b.PropertyType));
        }
        #endregion

        #region public instance methods
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented, converter);
        }

        public void Deserialize(string serialized)
        {
            BindableData deserialized = GetDeserializedData(serialized, GetType());
            Deserialize(deserialized);
        }

        private void Deserialize(BindableData deserialized)
        {
            foreach (var property in deserialized.AssignableValues.Where(kvp => IsSerializableValue(kvp.Value.Property)))
            {
                AssignableValues[property.Key].SetValue(property.Value.GetValue());
            }

            foreach (var child in deserialized.Childs)
                Childs[child.Key].Deserialize(child.Value);
        }

        public virtual void Initialize()
        {
            InitializeValues();
            InitializeHandlers();
            InitializeChilds();
        }
        #endregion

        #region private static methods
        private static string GetNameAndParamsFromMethodInfo(MethodInfo methodInfo)
        {

            string parameters = string.Join(", ", methodInfo.GetParameters().Select(p => p.ParameterType.Name));
            return $"{methodInfo.Name}({parameters})";
        }

        private static bool CheckTypeTreeByPredicate(Type type, Func<Type, bool> predicate)
        {
            while (type != typeof(object))
            {
                if (predicate(type))
                {
                    return true;
                }
                type = type.BaseType;
            }
            return false;
        }

        private static bool IsEditorVisibilityAcceptable(PropertyInfo property, bool isVisibleInEditorRequired)
        {
            return !(isVisibleInEditorRequired && property.GetCustomAttribute<ShowInEditorAttribute>() == null);
        }

        private static bool IsEditorVisibilityAcceptable(MethodInfo method, bool isVisibleInEditorRequired)
        {
            return !(isVisibleInEditorRequired && method.GetCustomAttribute<ShowInEditorAttribute>() == null);
        }
        #endregion

        #region private instance methods
        private void InitializeHandlers()
        {
            var handlers = new Dictionary<string, MethodInfo>();

            var handlerInfos = GetBindableHandlers(GetType());

            foreach (var handler in handlerInfos)
            {
                handlers[GetNameAndParamsFromMethodInfo(handler)] = handler;
            }

            BindableHandlers = new ReadOnlyDictionary<string, MethodInfo>(handlers);
        }

        private void InitializeValues()
        {
            var values = new Dictionary<string, BindableValueInfo>();
            var assignableValues = new Dictionary<string, AssignableValueInfo>();

            var valueProperties = GetBindableValues(GetType());

            foreach (var p in valueProperties)
            {
                values[p.Name] = new BindableValueInfo(p, this);
                if (IsAssignableValue(p.PropertyType))
                    assignableValues[p.Name] = new AssignableValueInfo(p, this);
            }

            Values = new ReadOnlyDictionary<string, BindableValueInfo>(values);
            AssignableValues = new ReadOnlyDictionary<string, AssignableValueInfo>(assignableValues);
        }

        private void InitializeChilds()
        {
            var childs = new Dictionary<string, BindableData>();

            var childProperties = GetBindableDatas(GetType());

            foreach (var p in childProperties)
            {
                childs[p.Name] = p.GetValue(this) as BindableData;

                childs[p.Name].Initialize();
            }

            Childs = new ReadOnlyDictionary<string, BindableData>(childs);
        }
        #endregion
    }
}