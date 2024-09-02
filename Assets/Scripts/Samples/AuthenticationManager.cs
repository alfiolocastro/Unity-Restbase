
using UnityEngine;

using Restbase.Firebase.Auth;

using TMPro;

public class AuthenticationManager : MonoBehaviour
{

    [SerializeField]
    TMP_Text emailplaceholder;
    
    [SerializeField]
    TMP_Text passwordplaceholder;

    //[SerializeField]
    //TMP_Text results;

    public static bool ignoreAuth;


    public string Email;
    public string Password;

    private const string EmailKey = "UserEmail";
    private const string PasswordKey = "UserPassword";

    // check this to true to test unathenticated behaviours 
    public bool IgnoreAuth { get => ignoreAuth; set => ignoreAuth = value; }

    // All'inizio, carica i dati dallo storage locale
    private void Awake()
    {
        LoadData();
    }

    // Metodo per salvare l'email nello storage locale
    public void SetEmail(string newEmail)
    {
        Email = newEmail;
        PlayerPrefs.SetString(EmailKey, Email);
        PlayerPrefs.Save();
    }

    // Metodo per salvare la password nello storage locale
    public void SetPassword(string newPassword)
    {
        Password = newPassword;
        PlayerPrefs.SetString(PasswordKey, Password);
        PlayerPrefs.Save();
    }

    // Carica i dati dallo storage locale all'inizializzazione
    private void LoadData()
    {
        if (PlayerPrefs.HasKey(EmailKey))
        {
            Email = PlayerPrefs.GetString(EmailKey);
            if(string.IsNullOrEmpty(Email))
            {
                emailplaceholder.text = Email;
            }
        }

        if (PlayerPrefs.HasKey(PasswordKey))
        {
            Password = PlayerPrefs.GetString(PasswordKey);
            if (string.IsNullOrEmpty(Password))
            {
                passwordplaceholder.text = Password;
            }
        }
    }

    // Se vuoi anche un reset dei dati
    public void ResetData()
    {
        PlayerPrefs.DeleteKey(EmailKey);
        PlayerPrefs.DeleteKey(PasswordKey);
        PlayerPrefs.Save();

        Email = string.Empty;
        Password = string.Empty;
    }

    public async void Authenticate()
    {
        bool signInSuccess = await FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(Email, Password)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    Debug.Log("Auth done! -> " + task.Result);
                }
                else
                {
                    Debug.LogError("Auth failed -> " + task.Result);
                }
            });


        if (signInSuccess)
        {
            //results.text = "Auth successfully";
        }
        else
        {
            //results.text = "Auth failed";
        }
    }

    public static bool IsLoggedIn()
    {
        if (ignoreAuth)
            return true;

        if (FirebaseAuth.DefaultInstance == null)
        {
            // log something
            Debug.Log("no user logged in");
            return false;
        }

        if (FirebaseAuth.DefaultInstance.CurrentUser == null)
        {
            // log something
            Debug.Log("no user logged in");
            return false;
        }

        return true;

    }

}
