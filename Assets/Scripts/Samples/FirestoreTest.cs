using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;
using Restbase.Firebase.Auth;
using Restbase.Firebase.Database;
using Restbase.Firebase.Storage;
public class FirestoreTest : MonoBehaviour
{

    [SerializeField] string email = "your-email@example.com";
    [SerializeField] string password = "your-password";

    async void Start()
    {
        // Autenticazione
        bool signInSuccess = await FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    Debug.Log("Auth done! ");
                }
                else
                {
                    Debug.LogError("Auth failed.");
                }
            });

        if (signInSuccess)
        {
            // Creare un riferimento alla collezione
            var collectionRef = new DatabaseReference("users");

            // Aggiungere un documento
            var userId = await collectionRef.AddDocumentAsync(new { name = "John Doe", age = 30, hobbies = new string[] { "Football", "Chess" } })
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        Debug.Log($"Documento aggiunto con ID: {task.Result}");
                    }
                    else
                    {
                        Debug.LogError("Errore durante l'aggiunta del documento.");
                    }
                });

            //return;

            // Ottenere un riferimento a un documento
            var docRef = collectionRef.Document(userId);

            /*
            // Impostare i dati del documento
            await docRef.SetAsync(new { name = "John Doe", age = 31 })
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        Debug.Log("Documento impostato con successo!");
                    }
                    else
                    {
                        Debug.LogError("Errore durante l'impostazione del documento.");
                    }
                });
            */

            // Aggiornare i dati del documento
            var updates = new Dictionary<string, object> { { "age", 32 } };
            await docRef.UpdateAsync(updates)
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        Debug.Log("Documento aggiornato con successo!");
                    }
                    else
                    {
                        Debug.LogError("Errore durante l'aggiornamento del documento.");
                    }
                });

            // Ottenere uno snapshot del documento
            await docRef.GetSnapshotAsync()
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        var snapshot = task.Result;
                        if (snapshot.Exists)
                        {
                            var data = snapshot.ConvertTo<UserData>();
                            Debug.Log($"Document retrieved, nome: {data.name}");
                        }
                        else
                        {
                            Debug.Log("Documento non esiste.");
                        }
                    }
                    else
                    {
                        Debug.LogError("Errore durante il recupero del documento.");
                    }
                });

            return;

            // Eliminare il documento
            await docRef.DeleteAsync()
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        Debug.Log("Document deleted!");
                    }
                    else
                    {
                        Debug.LogError("Document delete error");
                    }
                });


            // Esempio di caricamento di un file
            var storageReference = FirebaseStorage.DefaultInstance.GetReference().Child("uploads").Child("sampleupload.txt");
            await storageReference.PutFileAsync(Application.streamingAssetsPath + "/sampleupload.txt").ContinueWith(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    Debug.Log("File uploaded!");
                }
                else
                {
                    Debug.LogError("Upload error");
                }
            });

            // Esempio di download di un file
            await storageReference.GetFileAsync(Application.persistentDataPath + "/downloaded_example.txt").ContinueWith(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    Debug.Log($"File downloaded in {task.Result}");
                }
                else
                {
                    Debug.LogError("Download error!");
                }
            });


        }
    }
}
