using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Restbase.Firebase.Auth;
using Restbase.Firebase.Database;
using Restbase.Firebase.Storage;

public class DBManager : MonoBehaviour
{

    string UserId;

    void Start()
    {
        
    }


    void Update()
    {
        
    }

    public async void AddUser() {

        if (!AuthenticationManager.IsLoggedIn())
            return;

        // Creare un riferimento alla collezione
        var collectionRef = new DatabaseReference("users");

        // Aggiungere un documento
        var userId = await collectionRef.AddDocumentAsync(new { name = "John Doe", age = 30, hobbies = new string[] { "Football", "Chess" } })
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    Debug.Log($"Document user added, ID: {task.Result}");
                }
                else
                {
                    Debug.LogError("Error adding user document.");
                }
            });

        UserId = userId; 

    }

    // updates last added user
    public async void UpdateUser()
    {
        if (!AuthenticationManager.IsLoggedIn())
            return;

        var collectionRef = new DatabaseReference("users");
        var docRef = collectionRef.Document(UserId);

        // Aggiornare i dati del documento
        var updates = new Dictionary<string, object> { { "age", 32 } };
        await docRef.UpdateAsync(updates)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    Debug.Log("user updated!");
                }
                else
                {
                    Debug.LogError("Error updating.");
                }
            });
    }

    // sets last added user
    public async void SetUser()
    {
        if (!AuthenticationManager.IsLoggedIn())
            return;

        var collectionRef = new DatabaseReference("users");
        var docRef = collectionRef.Document(UserId);

        await docRef.SetAsync(new { name = "John Doe", age = 31 })
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    Debug.Log("Document sat!");
                }
                else
                {
                    Debug.LogError("Error setting document.");
                }
            });
    }

    public async void GetUser()
    {
        if (!AuthenticationManager.IsLoggedIn())
            return;

        var collectionRef = new DatabaseReference("users");
        var docRef = collectionRef.Document(UserId);

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
                        Debug.Log("Document does not exist.");
                    }
                }
                else
                {
                    Debug.LogError("Error retrieving document.");
                }
            });
    }



    // deletes last added user
    public async void DeleteUser()
    {

        if (!AuthenticationManager.IsLoggedIn())
            return;

        var collectionRef = new DatabaseReference("users");
        var docRef = collectionRef.Document(UserId);

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


    }


}
