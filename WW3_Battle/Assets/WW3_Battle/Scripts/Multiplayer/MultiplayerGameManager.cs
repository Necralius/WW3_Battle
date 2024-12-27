using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Random = UnityEngine.Random;

public class MultiplayerGameManager : MonoBehaviourPunCallbacks
{
    #region - Singleton Pattern -
    public static MultiplayerGameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    [SerializeField] private bool _debug = true;
    [SerializeField] private bool _connectedAndReady = false;

    public bool Connected
    {
        get
        {
            if (PhotonNetwork.IsConnected)
                return _connectedAndReady = PhotonNetwork.IsConnected;
            else 
                return ConnectToServer();
        }
    }

    public bool ConnectToServer()
    {
        if (!PhotonNetwork.IsConnected)
        {
            if (_debug) Debug.Log("Not connected! Connecting to master server...");
            return PhotonNetwork.ConnectUsingSettings();
        }
        else 
            return true;
    }

    #region - Callbacks -
    public override void OnConnectedToMaster()
    {
        if (_debug) 
            Debug.Log("Connected to server!");
        GameManager.Instance?.LayoutManager?.OpenPanel("Lobby");
        PhotonNetwork.NickName = $"{SessionManager.Instance.SessionData.Username}";

        PhotonNetwork.JoinLobby();
        base.OnConnectedToMaster();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (_debug)
            Debug.Log($"Disconnected from server, reason: {cause.ToString()}");

        PhotonNetwork.NickName = $"{SessionManager.Instance.SessionData.Username}";
        base.OnDisconnected(cause);
    }

    public override void OnJoinedLobby()
    {
        if (_debug) 
            Debug.Log("Joined on game lobby!");
        base.OnJoinedLobby();
    }

    public override void OnJoinedRoom()
    {
        if (_debug)
            Debug.Log($"Joined on room: {PhotonNetwork.CurrentRoom.Name}!");
        PhotonNetwork.AutomaticallySyncScene = true;

        base.OnJoinedRoom();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (GameManager.Instance.InGame && PhotonNetwork.CurrentRoom?.Players.Count < 2)
        {
            PhotonNetwork.LoadLevel(0);
            PhotonNetwork.LeaveRoom();
            GameManager.Instance.InGame = false;
        }

        base.OnPlayerLeftRoom(otherPlayer);
    }

    public override void OnLeftRoom()
    {
        if (_debug)
            Debug.Log($"Player has left from room!");
        PhotonNetwork.AutomaticallySyncScene = false;

        if (GameManager.Instance.InGame)
            GameManager.Instance.InGame = false;

        base.OnLeftRoom();
    }
    #endregion

    #region - Commands -
    public void StartGame()
    {
        if (Connected && PhotonNetwork.InRoom)
        {
            Debug.Log(PhotonNetwork.CurrentRoom.Players.Count);
            if (PhotonNetwork.CurrentRoom.Players.Count < 2)
            {
                Debug.Log("Need to have at least 2 players!");
                return;
            }
            else
            {
                GameManager.Instance.InGame = true;
                PhotonNetwork.LoadLevel(1);
            }
        }
    }

    #endregion
}