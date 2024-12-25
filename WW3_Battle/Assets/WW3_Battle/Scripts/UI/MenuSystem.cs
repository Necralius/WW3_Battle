using UnityEngine;

public class MenuSystem : MonoBehaviour
{

    [SerializeField] private UserAuthenticator _authenticator = null;

    private UserAuthenticator Authenticator
    {
        get
        {
            if (_authenticator == null)
                return _authenticator = FindFirstObjectByType<UserAuthenticator>();
            else return _authenticator;
        }
    }

    public void CreateRoom()
    {

    }

    public void FindRoom()
    {

    }

    public void Logout() => Authenticator.Logout();

    public void Quit()
    {
        Application.Quit();
    }
}