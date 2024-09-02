using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Restbase.Firebase.Firestore
{
    public class FieldPath
    {
        private readonly string _path;

        public FieldPath(params string[] segments)
        {
            _path = string.Join(".", segments);
        }

        public override string ToString() => _path;
    }
}