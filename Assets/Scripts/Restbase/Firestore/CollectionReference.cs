using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Restbase.Firebase.Auth;

namespace Restbase.Firebase.Firestore
{
    public class CollectionReference : Query
    {
        public CollectionReference(string path) : base(path)
        {
        }

        public DocumentReference Document(string documentId)
        {
            return new DocumentReference($"{_path}/{documentId}");
        }

        public async Task AddDocumentAsync<T>(T value)
        {
            if (!FirebaseAuth.DefaultInstance.IsAuthenticated)
            {
                await FirebaseAuth.DefaultInstance.RefreshTokenAsync();
            }

            string idToken = FirebaseAuth.DefaultInstance.GetIdToken();
            string url = $"https://firestore.googleapis.com/v1/projects/{FirebaseConfig.ProjectId}/databases/(default)/documents{_path}?documentId={Guid.NewGuid()}";

            string jsonData = JObject.FromObject(value).ToString();
            UnityWebRequest request = new UnityWebRequest(url, "POST")
            {
                uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData)),
                downloadHandler = new DownloadHandlerBuffer()
            };

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + idToken);

            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error adding document: {request.error}");
                throw new Exception("Failed to add document");
            }
            else
            {
                Debug.Log("Document added successfully.");
            }
        }
    }
}
