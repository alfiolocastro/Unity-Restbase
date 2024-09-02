using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

namespace Restbase.Firebase.Auth
{
    public class FirebaseAuth
    {
        private static FirebaseAuth _instance;
        public static FirebaseAuth DefaultInstance
        {
            get
            {
                if (_instance == null)
                {
                    UnityMainThreadDispatcher.Instance();
                    _instance = new FirebaseAuth();
                }
                return _instance;
            }
        }

        private string idToken;
        private string refreshToken;
        private string localId;
        private DateTime tokenExpiryTime;

        public bool IsAuthenticated => !string.IsNullOrEmpty(idToken) && tokenExpiryTime > DateTime.UtcNow;

        private FirebaseUser currentUser;
        public FirebaseUser CurrentUser
        {
            get { return currentUser; }
        }

        public async Task<bool> SignInWithEmailAndPasswordAsync(string email, string password)
        {
            string url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={FirebaseConfig.ApiKey}";

            var requestData = new
            {
                email,
                password,
                returnSecureToken = true
            };
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);
            //string json = JsonUtility.ToJson(requestData);

            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                var operation = request.SendWebRequest();
                while (!operation.isDone)
                    await Task.Yield();

                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError($"Error signing in: {request.error}");
                    Debug.LogError($"Error signing in: {request.downloadHandler.text}");
                    return false;
                }
                else
                {
                    JObject jsonResponse = JObject.Parse(request.downloadHandler.text);
                    idToken = jsonResponse["idToken"].ToString();
                    refreshToken = jsonResponse["refreshToken"].ToString();
                    localId = jsonResponse["localId"].ToString();
                    tokenExpiryTime = DateTime.UtcNow.AddSeconds(int.Parse(jsonResponse["expiresIn"].ToString()));
                    PlayerPrefs.SetString("UserId", localId);
                    PlayerPrefs.SetString("idToken", idToken);
                    PlayerPrefs.SetString("refreshToken", refreshToken);

                    currentUser = new FirebaseUser(_instance);
                    currentUser.UserId = localId;
                    currentUser.Email = jsonResponse["email"].ToString();
                    currentUser.DisplayName = jsonResponse["displayName"].ToString();
                    currentUser.IsAnonymous = false; // how do I check if a user is anonymous?
                    Debug.Log($"Auth successfull!");
                    return true;
                }
            }
        }

        public async Task<bool> RefreshTokenAsync()
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                Debug.LogError("No refresh token available.");
                return false;
            }

            string url = $"https://securetoken.googleapis.com/v1/token?key={FirebaseConfig.ApiKey}";

            var requestData = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "refresh_token", refreshToken }
        };

            using (UnityWebRequest request = UnityWebRequest.Post(url, requestData))
            {
                request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

                var operation = request.SendWebRequest();
                while (!operation.isDone)
                    await Task.Yield();

                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError($"Error refreshing token: {request.error}");
                    return false;
                }
                else
                {
                    JObject jsonResponse = JObject.Parse(request.downloadHandler.text);
                    idToken = jsonResponse["id_token"].ToString();
                    refreshToken = jsonResponse["refresh_token"].ToString();
                    tokenExpiryTime = DateTime.UtcNow.AddSeconds(int.Parse(jsonResponse["expires_in"].ToString()));
                    PlayerPrefs.SetString("idToken", idToken);
                    PlayerPrefs.SetString("refreshToken", refreshToken);
                    return true;
                }
            }
        }

        public string GetIdToken()
        {
            if (IsAuthenticated)
            {
                return idToken;
            }
            else
            {
                throw new Exception("User is not authenticated.");
            }
        }

        public void SignOut()
        {
            if (IsAuthenticated)
            {
                currentUser = null;
                idToken = null;
                refreshToken = null;
                PlayerPrefs.SetString("idToken", idToken);
                PlayerPrefs.SetString("refreshToken", refreshToken);
                PlayerPrefs.SetString("UserId", localId);
            }
        }
    }
}