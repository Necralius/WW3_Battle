using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OnlineRoomCreator : MonoBehaviour
{
    [SerializeField] private Button _createButton = null;

    [SerializeField] private TMP_InputField _roomName       = null;
    [SerializeField] private TMP_InputField _roomMaxPlayers = null;

    private void Start()
    {
        _createButton.onClick.RemoveAllListeners();
        _createButton.onClick.AddListener(() => CreateRoom());
    }

    public void CreateRoom()
    {
        if (MultiplayerGameManager.Instance.Connected)
        {
            if (VerifyRoomSettings())
            {
                PhotonNetwork.CreateRoom(_roomName.text, new Photon.Realtime.RoomOptions
                {
                    MaxPlayers = int.Parse(_roomMaxPlayers.text),
                    EmptyRoomTtl = 10000
                });
            }
        }
    }

    public bool VerifyRoomSettings()
    {
        if (_roomName.text == "" || _roomName.text == string.Empty)
        {
            _roomName.text = "Invalid room name!!!";
            return false;
        }

        int maxPlayers = 0;

        if (!int.TryParse(_roomMaxPlayers.text, out maxPlayers))
        {
            _roomMaxPlayers.text = "Must be above 8 and under 16 players!!!";
            return false;
        }

        if (maxPlayers < 8 || maxPlayers > 16)
        {
            _roomMaxPlayers.text = "Must be above 8 and under 16 players!!!";
            return false;
        }

        return true;
    }
}