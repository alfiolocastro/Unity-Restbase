using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using Restbase.Firebase.Auth;
using Restbase.Firebase.Firestore;
using Restbase.Utils;

namespace Restbase.Firebase.Database
{
    public class DatabaseReference : Query
    {
        public DatabaseReference(string path) : base(path) { }

        // Add a document to collection
        public async Task<string> AddDocumentAsync<T>(T documentData)
        {
            if (!FirebaseAuth.DefaultInstance.IsAuthenticated)
            {
                await FirebaseAuth.DefaultInstance.RefreshTokenAsync();
            }

            string idToken = FirebaseAuth.DefaultInstance.GetIdToken();
            string url = $"https://firestore.googleapis.com/v1/projects/{FirebaseConfig.ProjectId}/databases/(default)/documents/{_path}";


            // Converti il documentData in un formato compatibile con Firestore
            var fields = Converters.ConvertToFirestoreFormat(documentData);
            var jsonData = new JObject { ["fields"] = fields }.ToString();

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
                Debug.LogError($"Error adding document: {request.downloadHandler.text}");
                throw new Exception("Failed to add document");
            }

            var response = JObject.Parse(request.downloadHandler.text);
            return response["name"].ToString().Split('/').Last();

        }

        // Get a specific id reference [should stay here?]
        public DocumentReference Document(string documentId)
        {
            return new DocumentReference($"{_path}/{documentId}");
        }


    }

}