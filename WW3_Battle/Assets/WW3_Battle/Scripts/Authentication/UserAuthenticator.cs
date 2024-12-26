using TMPro;
using UnityEngine;
using Firebase.Extensions;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Events;

public class UserAuthenticator : MonoBehaviour
{
    [SerializeField] private TMP_InputField _emailLogin;
    [SerializeField] private TMP_InputField _passwordLogin;

    [SerializeField] private TMP_InputField _emailRegister;
    [SerializeField] private TMP_InputField _usernameRegister;
    [SerializeField] private TMP_InputField _passwordRegister;

    [SerializeField] private TMP_InputField _emailReset;


    [SerializeField] private UnityEvent _onSuccessfullLogin = new UnityEvent();

    private FirebaseApp  app;
    private FirebaseAuth auth;
    private DatabaseReference databaseReference;

    [SerializeField] private LayoutManager _layoutManager { get => GameManager.Instance?.LayoutManager; }

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            app  = FirebaseApp.DefaultInstance;
            auth = FirebaseAuth.DefaultInstance;

            if (task.Result == DependencyStatus.Available)
            {
                app.Options.DatabaseUrl = new System.Uri("https://ww3-battle-default-rtdb.firebaseio.com/");
                databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

                Debug.Log("Firebase Auth Initialized.");
                Debug.Log("Firebase initialized with Database URL.");
            }
            else
            {
                Debug.LogError($"Could not resolve Firebase dependencies: {task.Result}");
            }
        });
    }

    public void Login()
    {
        _layoutManager?.OpenPanel("Loading");
        auth?.SignInWithEmailAndPasswordAsync(_emailLogin?.text, _passwordLogin?.text).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("Login task was canceled.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("Login task encountered an error: " + task.Exception);
                Debug.Log(task.Result.AdditionalUserInfo);
                Debug.Log(task.Result);
                GameManager.Instance.LayoutManager.OpenPanel("Login");
                return;
            }

            AuthResult result = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);

            _onSuccessfullLogin?.Invoke();

            MultiplayerGameManager.Instance.ConnectToServer();

            GetUsername(result.User.UserId);
        });
    }

    public void Register()
    {
        _layoutManager?.OpenPanel("Loading");

        Debug.Log("Triggering register!");
        auth.CreateUserWithEmailAndPasswordAsync(_emailRegister.text, _passwordRegister.text).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("User regist task encountered an error: " + task.Exception.ToString());
                return;
            }

            AuthResult   result  = task.Result;
            FirebaseUser newUser = result.User;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})", result.User.DisplayName, result.User.UserId);

            _layoutManager?.OpenPanel("Login");

            SaveUsername(newUser.UserId);
        });
    }

    public void ResetPassword()
    {
        _layoutManager.OpenPanel("Loading");

        auth.SendPasswordResetEmailAsync(_emailReset.text).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("Password reset was canceled.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError($"Error while sending password reset email: {task.Exception}");
                return;
            }

            Debug.Log($"Password reset email sent successfully to: {_emailRegister.text}");
            _layoutManager.OpenPanel("Login");

        });
    }

    public void Logout()
    {
        auth.SignOut();
        _layoutManager.OpenPanel("Login");
    }

    public void SaveUsername(string userId)
    {
        databaseReference.Child("users").Child(userId).Child("username").SetValueAsync(_usernameRegister.text).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log($"Username: {_usernameRegister.text} saved successfully!");
            }
            else
            {
                Debug.LogError("Failed to save username: " + task.Exception);
            }
        });

        GetUsername(userId);
    }

    public void GetUsername(string userId)
    {
        Debug.Log("Trying to get username by id: " + userId);

        FirebaseDatabase.DefaultInstance.GetReference("users").OrderByChild("userId").EqualTo(userId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            Debug.Log("Task going on!");

            if (task.IsCompleted)
            {
                Debug.Log("Task completed!");
                DataSnapshot snapshot = task.Result;

                if (snapshot.Exists)
                {
                    foreach (DataSnapshot childSnapshot in snapshot.Children)
                    {
                        string username = childSnapshot.Child("username").Value.ToString();
                        Debug.Log("Username: " + username);
                    }
                }
            }
            else
            {
                Debug.LogError("Failed to retrieve username: " + task.Exception);
            }
        });
    }
}