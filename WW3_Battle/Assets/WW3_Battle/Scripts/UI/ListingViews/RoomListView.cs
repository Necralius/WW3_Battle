using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class RoomListView : MonoBehaviour
{
    [SerializeField] private GameObject       _roomPrefab = null;
    [SerializeField] private List<GameObject> _rooms      = null;
    [SerializeField] private Transform        _content    = null;

    public void UpdateContent(List<RoomData> players)
    {
        TransformUtils.UpdateSpawnedObjects(players, _rooms, _roomPrefab, _content);

        for (int i = 0; i < players.Count; i++)
            _rooms[i].GetComponent<RoomView>().SetUp(players[i]);
    }
}