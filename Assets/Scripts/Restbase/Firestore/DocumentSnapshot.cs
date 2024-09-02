using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Linq;

namespace Restbase.Firebase.Firestore
{
    public class DocumentSnapshot
    {
        private readonly JObject _data;

        public DocumentSnapshot(JObject data)
        {
            _data = data;
        }

        public string Id => _data["name"]?.ToString().Split('/').Last();

        public bool Exists => _data != null && _data["fields"] != null;

        public T ConvertTo<T>()
        {
            if (Exists)
            {
                string jsonData = _data["fields"].ToString();
                return JsonConvert.DeserializeObject<T>(jsonData);
            }

            return default;
        }

        public T GetValue<T>(string path)
        {
            if (Exists)
            {
                var field = GetFieldByPath(path);
                if (field != null)
                {
                    return field.ToObject<T>();
                }
            }

            return default;
        }

        public bool TryGetValue<T>(string path, out T value)
        {
            if (Exists)
            {
                var field = GetFieldByPath(path);
                if (field != null)
                {
                    value = field.ToObject<T>();
                    return true;
                }
            }

            value = default;
            return false;
        }

        public T GetValue<T>(FieldPath path)
        {
            if (Exists)
            {
                var field = GetFieldByPath(path.ToString());
                if (field != null)
                {
                    return field.ToObject<T>();
                }
            }

            return default;
        }

        public bool TryGetValue<T>(FieldPath path, out T value)
        {
            if (Exists)
            {
                var field = GetFieldByPath(path.ToString());
                if (field != null)
                {
                    value = field.ToObject<T>();
                    return true;
                }
            }

            value = default;
            return false;
        }

        public bool ContainsField(string path)
        {
            if (Exists)
            {
                var field = GetFieldByPath(path);
                return field != null;
            }

            return false;
        }

        private JToken GetFieldByPath(string path)
        {
            var segments = path.Split('.');
            JToken current = _data["fields"];

            foreach (var segment in segments)
            {
                if (current[segment] != null)
                {
                    current = current[segment];
                }
                else
                {
                    return null;
                }
            }

            return current;
        }
    }

}
