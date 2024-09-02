using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Restbase.Firebase.Firestore;
using Restbase.Firebase.Auth;

namespace Restbase.Firebase
{
    public class Query
    {
        protected readonly string _path;
        private readonly List<string> _orderByFields = new List<string>();
        private readonly List<string> _filters = new List<string>();

        public Query(string path)
        {
            _path = path;
        }

        public Query OrderBy(string field)
        {
            _orderByFields.Add(field);
            return this;
        }

        public Query Where(string field, string op, string value)
        {
            _filters.Add($"{field}{op}{value}");
            return this;
        }

        // Metodo generico per ottenere i documenti e deserializzarli in un tipo T
        public async Task<QuerySnapshot> GetSnapshotAsync()
        {
            if (!FirebaseAuth.DefaultInstance.IsAuthenticated)
            {
                await FirebaseAuth.DefaultInstance.RefreshTokenAsync();
            }

            string idToken = FirebaseAuth.DefaultInstance.GetIdToken();
            string url = $"https://firestore.googleapis.com/v1/projects/{FirebaseConfig.ProjectId}/databases/(default)/documents{_path}";

            List<string> queryParams = new List<string>();

            if (_orderByFields.Count > 0)
            {
                queryParams.Add($"orderBy={string.Join(",", _orderByFields)}");
            }

            if (_filters.Count > 0)
            {
                foreach (var filter in _filters)
                {
                    queryParams.Add($"filter={filter}");
                }
            }

            if (queryParams.Count > 0)
            {
                url += "?" + string.Join("&", queryParams);
            }

            UnityWebRequest request = new UnityWebRequest(url, "GET")
            {
                downloadHandler = new DownloadHandlerBuffer()
            };

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + idToken);

            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error getting documents: {request.error}");
                return null;
            }
            else
            {
                JObject jsonResponse = JObject.Parse(request.downloadHandler.text);
                List<DocumentSnapshot> documents = new List<DocumentSnapshot>();

                if (jsonResponse["documents"] != null)
                {
                    foreach (var doc in jsonResponse["documents"])
                    {
                        documents.Add(new DocumentSnapshot((JObject)doc));
                    }
                }

                return new QuerySnapshot(documents);
            }
        }
    }


}