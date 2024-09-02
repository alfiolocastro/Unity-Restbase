using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Restbase.Firebase.Auth;
using Restbase.Firebase.Database;
using Restbase.Firebase.Storage;

public class StorageManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async void UploadFile() {

        if (!AuthenticationManager.IsLoggedIn())
            return;

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

    }

    public async void DownloadFile()
    {
        if (!AuthenticationManager.IsLoggedIn())
            return;

        var storageReference = FirebaseStorage.DefaultInstance.GetReference().Child("uploads").Child("sampleupload.txt");
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
