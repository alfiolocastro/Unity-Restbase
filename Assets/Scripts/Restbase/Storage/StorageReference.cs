using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Restbase.Firebase.Auth;

namespace Restbase.Firebase.Storage
{
    public class StorageReference
    {
        private readonly string _path;

        public StorageReference(string path)
        {
            _path = path;
        }

        public StorageReference Child(string path)
        {
            return new StorageReference($"{_path}/{path}");
        }

        public Task<string> PutFileAsync(string localFilePath)
        {
            return UploadFileAsync(localFilePath);
        }

        private async Task<string> UploadFileAsync(string localFilePath)
        {
            // Verifica se l'utente è autenticato
            if (!FirebaseAuth.DefaultInstance.IsAuthenticated)
            {
                await FirebaseAuth.DefaultInstance.RefreshTokenAsync();
            }

            // Ottieni il token di autenticazione
            string idToken = FirebaseAuth.DefaultInstance.GetIdToken();

            // Leggi i byte del file
            byte[] fileData = File.ReadAllBytes(localFilePath);

            string sanitizedPath = _path.TrimStart('/');

            // Costruisci l'URL di Firebase Storage per l'upload multipart
            string url = $"https://firebasestorage.googleapis.com/v0/b/{FirebaseConfig.ProjectId}.appspot.com/o?uploadType=multipart&name={Uri.EscapeUriString(sanitizedPath)}";

            // Crea il contenuto multipart
            WWWForm form = new WWWForm();
            form.AddBinaryData("file", fileData, Path.GetFileName(localFilePath), "application/octet-stream");

            // Crea la richiesta HTTP
            UnityWebRequest request = UnityWebRequest.Post(url, form);
            request.SetRequestHeader("Authorization", "Bearer " + idToken);

            // Invia la richiesta
            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            // Gestisci la risposta
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error uploading file: {request.error}");
                Debug.LogError($"Error uploading file: {request.downloadHandler.text}");
                throw new Exception("Failed to upload file");
            }
            else
            {
                Debug.Log("File uploaded successfully.");
                return request.downloadHandler.text;
            }
        }



        public Task<string> GetFileAsync(string localFilePath)
        {
            return DownloadFileAsync(localFilePath);
        }

        private async Task<string> DownloadFileAsync(string localFilePath)
        {
            if (!FirebaseAuth.DefaultInstance.IsAuthenticated)
            {
                await FirebaseAuth.DefaultInstance.RefreshTokenAsync();
            }

            // Rimuovi la barra iniziale se presente e codifica il percorso
            string sanitizedPath = Uri.EscapeDataString(_path.TrimStart('/'));

            string idToken = FirebaseAuth.DefaultInstance.GetIdToken();
            string url = $"https://firebasestorage.googleapis.com/v0/b/{FirebaseConfig.ProjectId}.appspot.com/o/{sanitizedPath}?alt=media";

            // Debug: Stampa l'URL per verificare la formattazione
            Debug.Log($"Download URL: {url}");

            UnityWebRequest request = new UnityWebRequest(url, "GET")
            {
                downloadHandler = new DownloadHandlerFile(localFilePath)
            };

            request.SetRequestHeader("Authorization", "Bearer " + idToken);

            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error downloading file: {request.error}");
                throw new Exception("Failed to download file");
            }
            else
            {
                Debug.Log("File downloaded successfully.");
                return localFilePath;
            }
        }


    }
}
