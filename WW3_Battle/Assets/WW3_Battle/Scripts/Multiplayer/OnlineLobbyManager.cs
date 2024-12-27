using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class OnlineLobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private RoomListView   _roomListView = null;
    [SerializeField] private PlayerListView _playerListView = null;

    public RoomListView RoomListView
    {
        get
        {
            if (_roomListView == null)
                _roomListView = FindFirstObjectByType<RoomListView>();
            else 
                return _roomListView;
            return _roomListView;
        }
    }

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

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        List<RoomData> data = new List<RoomData>();

        if (roomList.Count > 0)
            foreach(RoomInfo room in roomList)
                data.Add(new RoomData(room));

        RoomListView.UpdateContent(data);
        base.OnRoomListUpdate(roomList);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (newMasterClient.IsLocal)
            PlayerListView.SetOwner();
        base.OnMasterClientSwitched(newMasterClient);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined on room: {PhotonNetwork.CurrentRoom.Name}!");

        GameManager.Instance?.LayoutManager?.OpenPanel("InsideRoom");
        PlayerListView.SetUp(PhotonNetwork.CurrentRoom.Name);

        base.OnJoinedRoom();
    }

    public override void OnLeftRoom()
    {
        GameManager.Instance?.LayoutManager?.OpenPanel("Lobby");

        base.OnLeftRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        PlayerListView?.UpdateContent();

        base.OnPlayerEnteredRoom(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PlayerListView?.UpdateContent();

        base.OnPlayerLeftRoom(otherPlayer);
    }
}