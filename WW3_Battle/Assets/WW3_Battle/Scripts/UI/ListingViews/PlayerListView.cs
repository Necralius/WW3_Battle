using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _roomName    = null;
    [SerializeField] private Button          _leaveButton = null;
    [SerializeField] private Button          _startGameButton = null;

    [SerializeField] private GameObject         _playerPrefab   = null;
    [SerializeField] private List<GameObject>   _players        = null;
    [SerializeField] private Transform          _content        = null;
    
    private List<PlayerData> playersData = new List<PlayerData>();

    private void Start()
    {
        _leaveButton?.    onClick?.AddListener(() => LeftRoom());
        _startGameButton?.onClick?.AddListener(() => MultiplayerGameManager.Instance?.StartGame());
        
    }

    public void SetUp()
    {
        _roomName.text = PhotonNetwork.CurrentRoom.Name;

        UpdateContent();
    }

    public void SetOwner()
    {
        _startGameButton?.gameObject.SetActive(PhotonNetwork.LocalPlayer.IsMasterClient);
    }

    public void UpdateContent()
    {
        List<PlayerData> players = new List<PlayerData>();

        foreach (var player in PhotonNetwork.PlayerList) 
            players.Add(new PlayerData(player));

        TransformUtils.UpdateSpawnedObjects(players, _players, _playerPrefab, _content);

        for (int i = 0; i < players.Count; i++)
            _players[i].GetComponent<PlayerView>().SetUp(players[i]);
    }

    public void LeftRoom() => PhotonNetwork.LeaveRoom();
}