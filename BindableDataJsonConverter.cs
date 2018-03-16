using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Shibari
{

    internal class BindableDataJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(BindableData).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            BindableData instance = (BindableData)existingValue;
            if (instance == null)
            {
                instance = (BindableData)Activator.CreateInstance(objectType);
                instance.Initialize();
            }

            JObject jsonObject = JObject.Load(reader);
            foreach (var serialized in jsonObject.Properties())
            {
                if (instance.AssignableValues.ContainsKey(serialized.Name))
                {
                    AssignableValueInfo reflected = instance.AssignableValues[serialized.Name];
                    if (BindableData.IsSerializableValue(reflected.Property))
                    {
                        if (serialized.Value.Type == JTokenType.Array)
                        {
                            Type elementType = null;
                            if (reflected.ValueType.IsArray)
                            {
                                elementType = reflected.ValueType.GetElementType();
                            }
                            else if (reflected.ValueType.GetInterface(nameof(IDictionary)) != null)
                            {
                                elementType = typeof(KeyValuePair<,>).MakeGenericType(reflected.ValueType.GenericTypeArguments);
                            }
                            else if (reflected.ValueType.GetInterface(nameof(IEnumerable)) != null)
                            {
                                elementType = reflected.ValueType.GenericTypeArguments[0];
                            }

                            var elements = serialized.Values().Select(v => v.ToObject(elementType)).ToArray();
                            reflected.SetValue(elements);
                            continue;
                        }
                        reflected.SetValue(serialized.ToObject(reflected.ValueType));
                        continue;
                    }
                    else
                    {
                        Debug.LogError($"Property {serialized.Name} is not marked as serializable.");
                        continue;
                    }
                }
                else
                {
                    Debug.LogError($"Property {serialized.Name} is not found.");
                    continue;
                }
            }

            return instance;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JObject jsonObject = new JObject();
            BindableData data = (BindableData)value;

            foreach (var property in data.Values.Where(p => BindableData.IsSerializableValue(p.Value.Property)))
            {
                object v = property.Value.GetValue();
                if (v == null)
                {
                    if (property.Value.ValueType == typeof(string) || property.Value.ValueType.GetInterface(nameof(IEnumerable)) == null)
                    {
                        jsonObject.AddFirst(new JProperty(property.Key, new JValue("")));
                    }
                    else
                    {
                        jsonObject.AddFirst(new JProperty(property.Key, new JArray()));
                    }
                }
                else
                {
                    jsonObject.AddFirst(new JProperty(property.Key, JToken.FromObject(v)));
                }
            }

            jsonObject.WriteTo(writer);
        }

        public static string GenerateJsonTemplate(Type type)
        {
            if (!typeof(BindableData).IsAssignableFrom(type))
                throw new ArgumentException("Type should be child of BindableData", "type");

            JObject jsonObject = GenerateJsonObject(type);
            
            return jsonObject.ToString(Formatting.Indented);
        }

        private static JObject GenerateJsonObject(Type type)
        {
            JObject jsonObject = new JObject();

            foreach (var property in BindableData.GetSerializableValues(type))
            {
                Type valueType = BindableData.GetBindableValueValueType(property.PropertyType);

                if (valueType.IsValueType)
                {
                    jsonObject.Add(new JProperty(property.Name, Activator.CreateInstance(valueType)));
                }
                else if (valueType == typeof(string) || valueType.GetInterface(nameof(IEnumerable)) == null)
                {
                    jsonObject.Add(new JProperty(property.Name, new JValue("")));
                }
                else
                {
                    jsonObject.Add(new JProperty(property.Name, new JArray()));
                }
            }

            foreach (var property in BindableData.GetBindableDatas(type).Where(t => BindableData.HasSerializeableValuesInChilds(t.PropertyType)))
            {
                jsonObject.Add(new JProperty(property.Name, GenerateJsonObject(property.PropertyType)));
            }

            return jsonObject;
        }
    }
}