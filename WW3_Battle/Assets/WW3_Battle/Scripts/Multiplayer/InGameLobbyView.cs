using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameLobbyView : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button _respawn = null;
    [SerializeField] private Button _leftRoom = null;

    [SerializeField] private GameObject     _inGameLobbyView = null;
    [SerializeField] private PlayerListView _playerListView  = null;

    public PlayerListView PlayerListView
    {
        get
        {
            if (_playerListView == null)
                _playerListView = FindFirstObjectByType<PlayerListView>();
            else
                return _playerListView;
            return _playerListView;
        }
    }

    private void Start()
    {
        _respawn. onClick.RemoveAllListeners();
        _leftRoom.onClick.RemoveAllListeners();

        _respawn. onClick.AddListener(() => Respawn());
        _leftRoom.onClick.AddListener(() => LeftRoom());


        SetUp();
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene Loaded, Setting up infos!");
        SetUp();
    }

    public void ChangeState(bool state)
    {
        _inGameLobbyView.SetActive(state);
        if (state)
        {
            SetUp();
        }
    }

    public void SetUp()
    {
        RoomInfo room = null;
        if (MultiplayerGameManager.Instance.Connected
            && PhotonNetwork.InRoom)
            room = PhotonNetwork.CurrentRoom;

        if (room != null)
        {
            PlayerListView?.SetUp();
            PlayerListView?.UpdateContent();
        }
    }

    private void Respawn()
    {
        Debug.Log("Spawning!");


    }
    
    private void LeftRoom()
    {
        if (MultiplayerGameManager.Instance.Connected 
            && PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();

        Debug.Log("Loading Lobby Scene!");

        if (PhotonNetwork.CurrentRoom.PlayerCount < 2)
            PhotonNetwork.LoadLevel(0);
        else
            SceneManager.LoadScene(0);
    }

}