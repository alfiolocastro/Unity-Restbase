using UnityEngine;

public static class FirebaseConfig
{
    public static string ProjectId { get; private set; }
    public static string ApiKey { get; private set; }

    static FirebaseConfig()
    {
        // loads config info from google-services.json
        TextAsset configJson = Resources.Load<TextAsset>("google-services");
        var jsonConfig = JsonUtility.FromJson<GoogleServicesConfig>(configJson.text);
        ProjectId = jsonConfig.project_info.project_id;
        ApiKey = jsonConfig.client[0].api_key[0].current_key;

        Debug.Log($"Project id is {ProjectId} and API key is {ApiKey}");
    }

    [System.Serializable]
    private class GoogleServicesConfig
    {
        public ProjectInfo project_info;
        public Client[] client;

        [System.Serializable]
        public class ProjectInfo
        {
            public string project_id;
        }

        [System.Serializable]
        public class Client
        {
            public ApiKey[] api_key;
        }

        [System.Serializable]
        public class ApiKey
        {
            public string current_key;
        }
    }
}
