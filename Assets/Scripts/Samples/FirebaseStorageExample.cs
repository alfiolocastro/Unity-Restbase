using UnityEngine;
using Restbase.Firebase.Auth;
using Restbase.Firebase.Storage;

public class FirebaseStorageExample : MonoBehaviour
{
    void Start()
    {
        // Esempio di caricamento di un file
        var storageReference = FirebaseStorage.DefaultInstance.GetReference().Child("uploads").Child("example.txt");
        storageReference.PutFileAsync(Application.persistentDataPath + "/example.txt").ContinueWith(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                Debug.Log("File caricato con successo!");
            }
            else
            {
                Debug.LogError("Errore durante il caricamento del file.");
            }
        });

        // Esempio di download di un file
        storageReference.GetFileAsync(Application.persistentDataPath + "/downloaded_example.txt").ContinueWith(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                Debug.Log($"File scaricato con successo in {task.Result}");
            }
            else
            {
                Debug.LogError("Errore durante il download del file.");
            }
        });
    }
}
