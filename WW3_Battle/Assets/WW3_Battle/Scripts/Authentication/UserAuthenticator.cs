using TMPro;
using UnityEngine;
using Firebase.Extensions;
using Firebase.Auth;
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

    [SerializeField] private LayoutManager _layoutManager { get => GameManager.Instance?.LayoutManager; }

    private FirebaseAuth auth;
    public FirebaseAuth AuthApp { get => auth; set => auth = value; }

    public void Login()
    {
        _layoutManager?.OpenPanel("Loading");
        auth?.SignInWithEmailAndPasswordAsync(_emailLogin?.text, _passwordLogin?.text).ContinueWithOnMainThread(task =>
        {
            Debug.Log(_emailLogin.text + " " + _passwordLogin.text);
            if (task.IsCanceled || task.IsFaulted)
            {
                GameManager.Instance.LayoutManager.OpenPanel("Login");
                _emailLogin.   text = "Wrong username/email or password";
                _passwordLogin.text = "";
            }

            if (task.IsCompleted)
            {
                AuthResult result = task.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})",
                    result.User.DisplayName, result.User.UserId);

                _onSuccessfullLogin?.Invoke();
                SessionManager.Instance?.UpdateSessionData(result.User.UserId);

                MultiplayerGameManager.Instance?.ConnectToServer();
            }
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
                _emailRegister.     text = "Invalid credentials, try again!";
                _usernameRegister.  text = "";
                _passwordRegister.  text = "";
                _layoutManager?.OpenPanel("Register");
            }

            if (task.IsCompleted)
            {
                AuthResult   result  = task.Result;
                Debug.LogFormat("Firebase user created successfully: {0} ({1})", result.User.DisplayName, result.User.UserId);

                _layoutManager?.OpenPanel("Login");

                SessionManager.Instance?.SaveData(_usernameRegister.text, result.User.UserId, true);
            }
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
}