using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Restbase.Firebase.Firestore;
using Restbase.Firebase.Auth;

namespace Restbase.Firebase
{
    public class QuerySnapshot
    {
        private readonly List<DocumentSnapshot> _documents;

        public QuerySnapshot(List<DocumentSnapshot> documents)
        {
            _documents = documents;
        }

        public IEnumerable<DocumentSnapshot> Documents => _documents;
    }
}
