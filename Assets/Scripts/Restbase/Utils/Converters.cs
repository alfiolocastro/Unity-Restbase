using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Newtonsoft.Json;

namespace Restbase.Utils
{
    public class Converters
    {
        public static JObject ConvertToFirestoreFormat(object documentData)
        {
            JObject firestoreFields = new JObject();
            JObject jsonDocument = JObject.FromObject(documentData);

            foreach (var property in jsonDocument.Properties())
            {
                var value = property.Value;
                JObject firestoreValue = new JObject();

                switch (value.Type)
                {
                    case JTokenType.Integer:
                        firestoreValue["integerValue"] = value.ToString();
                        break;
                    case JTokenType.Float:
                        firestoreValue["doubleValue"] = value.ToString();
                        break;
                    case JTokenType.String:
                        firestoreValue["stringValue"] = value.ToString();
                        break;
                    case JTokenType.Boolean:
                        firestoreValue["booleanValue"] = value.ToString().ToLower();
                        break;
                    case JTokenType.Array:
                        var arrayElements = new JArray();
                        foreach (var item in value)
                        {
                            JObject elementFirestoreValue = new JObject();
                            switch (item.Type)
                            {
                                case JTokenType.Integer:
                                    elementFirestoreValue["integerValue"] = item.ToString();
                                    break;
                                case JTokenType.Float:
                                    elementFirestoreValue["doubleValue"] = item.ToString();
                                    break;
                                case JTokenType.String:
                                    elementFirestoreValue["stringValue"] = item.ToString();
                                    break;
                                case JTokenType.Boolean:
                                    elementFirestoreValue["booleanValue"] = item.ToString().ToLower();
                                    break;
                                case JTokenType.Object:
                                    elementFirestoreValue["mapValue"] = new JObject
                                    {
                                        ["fields"] = ConvertToFirestoreFormat(item)
                                    };
                                    break;
                                default:
                                    throw new NotSupportedException($"Type {item.Type} is not supported in array.");
                            }
                            arrayElements.Add(elementFirestoreValue);
                        }
                        firestoreValue["arrayValue"] = new JObject { ["values"] = arrayElements };
                        break;
                    case JTokenType.Object:
                        firestoreValue["mapValue"] = new JObject
                        {
                            ["fields"] = ConvertToFirestoreFormat(value)
                        };
                        break;
                    // Aggiungi altri tipi di Firestore supportati qui
                    default:
                        throw new NotSupportedException($"Type {value.Type} is not supported.");
                }

                firestoreFields[property.Name] = firestoreValue;
            }

            return firestoreFields;
        }



        public static T ConvertFromFirestoreFormat<T>(JObject firestoreDocument)
        {
            var result = Activator.CreateInstance<T>();
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                var firestoreField = firestoreDocument["fields"]?[property.Name];
                if (firestoreField != null)
                {
                    JToken valueToken = null;

                    if (firestoreField["stringValue"] != null)
                    {
                        valueToken = firestoreField["stringValue"];
                    }
                    else if (firestoreField["integerValue"] != null)
                    {
                        valueToken = firestoreField["integerValue"];
                    }
                    else if (firestoreField["doubleValue"] != null)
                    {
                        valueToken = firestoreField["doubleValue"];
                    }
                    else if (firestoreField["booleanValue"] != null)
                    {
                        valueToken = firestoreField["booleanValue"];
                    }
                    else if (firestoreField["arrayValue"] != null)
                    {
                        var arrayItems = firestoreField["arrayValue"]["values"] as JArray;
                        var elementType = property.PropertyType.GetElementType() ?? property.PropertyType.GenericTypeArguments.First();
                        var list = Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType)) as IList;

                        foreach (var item in arrayItems)
                        {
                            var itemValue = ConvertToElement(elementType, item);
                            list.Add(itemValue);
                        }
                        valueToken = JToken.FromObject(list);
                    }
                    else if (firestoreField["mapValue"] != null)
                    {
                        var map = firestoreField["mapValue"]["fields"] as JObject;
                        var mapType = property.PropertyType;
                        var dictionary = Activator.CreateInstance(mapType) as IDictionary;

                        foreach (var mapProperty in map.Properties())
                        {
                            var key = mapProperty.Name;
                            var mapValue = ConvertToElement(mapType.GenericTypeArguments[1], mapProperty.Value);
                            dictionary.Add(key, mapValue);
                        }
                        valueToken = JToken.FromObject(dictionary);
                    }
                    // Add support for other Firestore types here

                    if (valueToken != null)
                    {
                        var convertedValue = valueToken.ToObject(property.PropertyType);
                        property.SetValue(result, convertedValue);
                    }
                }
            }

            return result;
        }

        // Aux Method simple types conversion
        public static object ConvertToElement(Type targetType, JToken firestoreValue)
        {
            if (firestoreValue["stringValue"] != null)
            {
                return Convert.ChangeType(firestoreValue["stringValue"].ToString(), targetType);
            }
            else if (firestoreValue["integerValue"] != null)
            {
                return Convert.ChangeType(firestoreValue["integerValue"].ToString(), targetType);
            }
            else if (firestoreValue["doubleValue"] != null)
            {
                return Convert.ChangeType(firestoreValue["doubleValue"].ToString(), targetType);
            }
            else if (firestoreValue["booleanValue"] != null)
            {
                return Convert.ChangeType(firestoreValue["booleanValue"].ToString(), targetType);
            }
            else if (firestoreValue["mapValue"] != null)
            {
                var mapType = typeof(Dictionary<,>).MakeGenericType(typeof(string), targetType);
                var mapDocument = firestoreValue["mapValue"]["fields"] as JObject;
                var mapObject = Activator.CreateInstance(mapType) as IDictionary;

                foreach (var mapProperty in mapDocument.Properties())
                {
                    var mapKey = mapProperty.Name;
                    var mapVal = ConvertToElement(targetType, mapProperty.Value);
                    mapObject.Add(mapKey, mapVal);
                }

                return mapObject;
            }
            else if (firestoreValue["arrayValue"] != null)
            {
                var arrayElements = firestoreValue["arrayValue"]["values"] as JArray;
                var elementType = targetType.GetElementType() ?? targetType.GenericTypeArguments.First();
                var list = Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType)) as IList;

                foreach (var item in arrayElements)
                {
                    list.Add(ConvertToElement(elementType, item));
                }

                return list;
            }

            throw new NotSupportedException($"Unsupported Firestore value type for {targetType.Name}");
        }

        public static JObject ConvertFromFirestoreFormat(string firestoreJson)
        {
            // Deserializza la stringa JSON in un JObject
            JObject firestoreDocument = JObject.Parse(firestoreJson);

            // Converte il contenuto di "fields" mantenendo la struttura originale
            firestoreDocument["fields"] = ConvertToJObject(firestoreDocument["fields"] as JObject);

            // Serializza l'oggetto convertito in una stringa JSON
            return firestoreDocument;
        }

        private static JObject ConvertToJObject(JObject firestoreFields)
        {
            var result = new JObject();

            foreach (var property in firestoreFields.Children<JProperty>())
            {
                var firestoreField = property.Value;
                JToken valueToken = null;

                if (firestoreField["stringValue"] != null)
                {
                    valueToken = firestoreField["stringValue"];
                }
                else if (firestoreField["integerValue"] != null)
                {
                    valueToken = firestoreField["integerValue"];
                }
                else if (firestoreField["doubleValue"] != null)
                {
                    valueToken = firestoreField["doubleValue"];
                }
                else if (firestoreField["booleanValue"] != null)
                {
                    valueToken = firestoreField["booleanValue"];
                }
                else if (firestoreField["arrayValue"] != null)
                {
                    var arrayElements = new JArray();
                    foreach (var item in firestoreField["arrayValue"]["values"])
                    {
                        // Verifica se l'elemento è un oggetto complesso o un tipo primitivo
                        if (item["stringValue"] != null)
                        {
                            arrayElements.Add(item["stringValue"]);
                        }
                        else if (item["integerValue"] != null)
                        {
                            arrayElements.Add(item["integerValue"]);
                        }
                        else if (item["doubleValue"] != null)
                        {
                            arrayElements.Add(item["doubleValue"]);
                        }
                        else if (item["booleanValue"] != null)
                        {
                            arrayElements.Add(item["booleanValue"]);
                        }
                        else if (item["mapValue"] != null)
                        {
                            arrayElements.Add(ConvertToJObject(item["mapValue"]["fields"] as JObject));
                        }
                        else if (item["arrayValue"] != null)
                        {
                            arrayElements.Add(ConvertToJObject(item["arrayValue"]["values"] as JObject));
                        }
                        else
                        {
                            throw new NotSupportedException($"Unsupported Firestore type in array: {item}");
                        }
                    }
                    valueToken = arrayElements;
                }
                else if (firestoreField["mapValue"] != null)
                {
                    var mapFields = firestoreField["mapValue"]["fields"] as JObject;
                    valueToken = ConvertToJObject(mapFields);
                }
                // Add support for other Firestore types here

                result[property.Name] = valueToken;
            }

            return result;
        }

    }
}