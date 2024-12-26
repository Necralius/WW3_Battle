using System;
using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _name  = null;   
    [SerializeField] private TextMeshProUGUI _ping  = null;   
    [SerializeField] private TextMeshProUGUI _level = null;   

    public void SetUp(PlayerData data)
    {
        _name.    text = data.Name;
        _level.   text = data.Level.ToString();
    }
}

[Serializable]
public class PlayerData
{
    public string Name  = "Player0000";
    public int    Level = 0;
    public bool   Ready = false;
    public string Role  = "_client"; //_client or _host

    public PlayerData(Player data)
    {
        Name  = data.NickName;
        Level = 0;
        Ready = false;
        Role  = data.IsMasterClient ? "_host" : "_client";
    }
}