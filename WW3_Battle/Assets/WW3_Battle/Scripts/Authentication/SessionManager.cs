using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;
using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
    #region - Singleton Pattern -
    public static SessionManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    #endregion

    [SerializeField] private UserSessionData _sessionData = null;
    [SerializeField] private string          _databaseUrl = "https://ww3-battle-default-rtdb.firebaseio.com/";

    private FirebaseApp app;
    private DatabaseReference databaseReference;

    public UserSessionData SessionData { get => _sessionData; }

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            app = FirebaseApp.DefaultInstance;

            UserAuthenticator authenticator = FindFirstObjectByType<UserAuthenticator>();
            authenticator.AuthApp = FirebaseAuth.DefaultInstance;

            if (task.Result == DependencyStatus.Available)
            {
                app.Options.DatabaseUrl = new Uri(_databaseUrl);
                databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

                Debug.Log("Firebase Auth Initialized.");
                Debug.Log("Firebase App Initialized.");
                Debug.Log($"Firebase Database Initialized with URL: {_databaseUrl}");
            }
            else
            {
                Debug.LogError($"Could not resolve Firebase dependencies: {task.Result}");
            }
        });
    }

    public void ClearSession()
    {
        _sessionData = null;

    }

    public void UpdateSessionData(string userId) => StartCoroutine(GetSessionData(userId));

    IEnumerator GetSessionData(string userId)
    {
        Debug.Log("Getting user data!");
        var serverData = databaseReference.Child("users").Child(userId).GetValueAsync();
        yield return new WaitUntil(predicate: () => serverData.IsCompleted);

        Debug.Log("Completed!");

        DataSnapshot snapshot = serverData.Result;
        string jsonData = snapshot.GetRawJsonValue();

        if (jsonData != null)
        {
            try
            {
                Debug.Log($"Data found: {jsonData}");
                _sessionData = JsonConvert.DeserializeObject<UserSessionData>(jsonData);

                PhotonNetwork.LocalPlayer.NickName = _sessionData.Username;
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }
        }
        else 
            Debug.LogWarning("Data not found!");
    }

    public void SaveData(string username, string userId, bool create = false)
    {
        string json = string.Empty;
        if (create)
        {
            _sessionData = new UserSessionData()
            {
                Username = username,
                UserId   = userId,
            };
        }

        json = JsonUtility.ToJson(_sessionData);
        databaseReference.Child("users").Child(userId).SetRawJsonValueAsync(json);
    }
}

[Serializable]
public class UserSessionData
{
    public string Username      = string.Empty;
    public string UserId        = string.Empty;
    public int    Level         = 0;
    public int    AllKills      = 0;
    public int    AllSessions   = 0;
}