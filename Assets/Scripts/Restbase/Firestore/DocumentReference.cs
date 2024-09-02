using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Restbase.Firebase.Auth;
using Restbase.Utils;

namespace Restbase.Firebase.Firestore
{
    public class DocumentReference
    {
        private readonly string _path;

        public DocumentReference(string path)
        {
            _path = path;
        }

        //delete document
        public async Task DeleteAsync()
        {
            if (!FirebaseAuth.DefaultInstance.IsAuthenticated)
            {
                await FirebaseAuth.DefaultInstance.RefreshTokenAsync();
            }

            string idToken = FirebaseAuth.DefaultInstance.GetIdToken();
            string url = $"https://firestore.googleapis.com/v1/projects/{FirebaseConfig.ProjectId}/databases/(default)/documents/{_path}";

            UnityWebRequest request = new UnityWebRequest(url, "DELETE")
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
                Debug.LogError($"Error deleting document: {request.error}");
                throw new Exception("Failed to delete document");
            }
            else
            {
                Debug.Log("Document deleted successfully.");
            }
        }

        // update document from selected updates
        public async Task UpdateAsync(IDictionary<string, object> updates)
        {
            if (!FirebaseAuth.DefaultInstance.IsAuthenticated)
            {
                await FirebaseAuth.DefaultInstance.RefreshTokenAsync();
            }

            string idToken = FirebaseAuth.DefaultInstance.GetIdToken();
            string url = $"https://firestore.googleapis.com/v1/projects/{FirebaseConfig.ProjectId}/databases/(default)/documents/{_path}?updateMask.fieldPaths={string.Join(",", updates.Keys)}";

            var fields = Converters.ConvertToFirestoreFormat(updates);
            var jsonData = new JObject { ["fields"] = fields }.ToString();
            //string jsonData = JObject.FromObject(new { fields = updates }).ToString();
            UnityWebRequest request = new UnityWebRequest(url, "PATCH")
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
                Debug.LogError($"Error updating document: {request.error}");
                throw new Exception("Failed to update document");
            }
            else
            {
                Debug.Log("Document updated successfully.");
            }
        }

        // update doc data
        public async Task UpdateAsync(IDictionary<FieldPath, object> updates)
        {
            if (!FirebaseAuth.DefaultInstance.IsAuthenticated)
            {
                await FirebaseAuth.DefaultInstance.RefreshTokenAsync();
            }

            string idToken = FirebaseAuth.DefaultInstance.GetIdToken();
            string url = $"https://firestore.googleapis.com/v1/projects/{FirebaseConfig.ProjectId}/databases/(default)/documents/{_path}?updateMask.fieldPaths={string.Join(",", updates.Keys)}";

            var updatesJson = new JObject();
            foreach (var update in updates)
            {
                updatesJson[update.Key.ToString()] = JToken.FromObject(update.Value);
            }

            string jsonData = JObject.FromObject(new { fields = updatesJson }).ToString();
            UnityWebRequest request = new UnityWebRequest(url, "PATCH")
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
                Debug.LogError($"Error updating document: {request.error}");
                throw new Exception("Failed to update document");
            }
            else
            {
                Debug.Log("Document updated successfully.");
            }
        }

        // set doc data
        public async Task SetAsync(object documentData, SetOptions options = null)
        {
            if (!FirebaseAuth.DefaultInstance.IsAuthenticated)
            {
                await FirebaseAuth.DefaultInstance.RefreshTokenAsync();
            }

            string idToken = FirebaseAuth.DefaultInstance.GetIdToken();
            string url = $"https://firestore.googleapis.com/v1/projects/{FirebaseConfig.ProjectId}/databases/(default)/documents/{_path}";

            var fields = Converters.ConvertToFirestoreFormat(documentData);
            var jsonData = new JObject { ["fields"] = fields }.ToString();
            UnityWebRequest request = new UnityWebRequest(url, "PATCH")
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
                Debug.LogError($"Error setting document: {request.error}");
                throw new Exception("Failed to set document");
            }
            else
            {
                Debug.Log("Document set successfully.");
            }
        }

        // Get a doc snapshot
        public async Task<DocumentSnapshot> GetSnapshotAsync(Source source = null)
        {
            if (!FirebaseAuth.DefaultInstance.IsAuthenticated)
            {
                await FirebaseAuth.DefaultInstance.RefreshTokenAsync();
            }

            string idToken = FirebaseAuth.DefaultInstance.GetIdToken();
            string url = $"https://firestore.googleapis.com/v1/projects/{FirebaseConfig.ProjectId}/databases/(default)/documents/{_path}";

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
                Debug.LogError($"Error getting document snapshot: {request.error}");
                return null;
            }
            else
            {
                //JObject jsonResponse = JObject.Parse(request.downloadHandler.text);
                //jsonResponse = Utils.ConvertFromFirestoreFormat<JObject>(jsonResponse);
                JObject jsonResponse = Converters.ConvertFromFirestoreFormat(request.downloadHandler.text);
                return new DocumentSnapshot(jsonResponse);
            }
        }
    }
}
