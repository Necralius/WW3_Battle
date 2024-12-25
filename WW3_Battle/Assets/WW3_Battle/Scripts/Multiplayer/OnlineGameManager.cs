using UnityEngine;
using Fusion;

public class OnlineGameManager : MonoBehaviour
{
    #region - Singleton Pattern -
    public static OnlineGameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    [SerializeField] private NetworkRunner _networkRunner = null;

    public NetworkRunner NetworkRunner
    {
        get
        {
            if (_networkRunner == null && TryGetComponent(out _networkRunner))
                return _networkRunner;
            else if (!GetComponent<NetworkRunner>())
                return _networkRunner = gameObject.AddComponent<NetworkRunner>();
            else return _networkRunner;
        }
    }

    public async void ConnectToServer()
    {
        var result = await NetworkRunner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "FusionSession01",
            PlayerCount = 10

        });

        if (result.Ok)
        {
            Debug.Log("Successfully connected to Photon Fusion server!");
        }
        else
        {
            Debug.LogError($"Failed to connect to Photon Fusion server: {result.ShutdownReason}");
        }
    }



}