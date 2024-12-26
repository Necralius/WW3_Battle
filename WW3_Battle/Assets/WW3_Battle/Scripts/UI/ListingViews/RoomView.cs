using Photon.Pun;
using Photon.Realtime;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class RoomView : MonoBehaviour
{
    private Button _btn => GetComponent<Button>();

    [SerializeField] private TextMeshProUGUI _name           = null;
    [SerializeField] private TextMeshProUGUI _playerQuantity = null;

    private RoomData _roomData = null;

    public void SetUp(RoomData data)
    {
        _name.          text = data.Name;
        _playerQuantity.text = data.PlayerQuantityFormated;

        _btn.onClick.RemoveAllListeners();
        _btn.onClick.AddListener(() => PhotonNetwork.JoinRoom(data.Name));
    }
}

[Serializable]
public class RoomData
{
    public string   Name     = "Room";
    public RoomInfo RoomInfo = null;

    public string PlayerQuantityFormated { get => $"{RoomInfo.PlayerCount}/{RoomInfo.MaxPlayers}"; }

    public RoomData(RoomInfo data)
    {
        Name     = data.Name;
        RoomInfo = data;
    }
}
