using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Map;

public class RoomManager : MonoBehaviour {

    private static RoomManager instance;
    List<Map.Rect> roomList;

    public static RoomManager Getinstance()
    {
        if (instance == null)
        {
            instance = GameObject.FindObjectOfType(typeof(RoomManager)) as RoomManager;
        }
        return instance;
    }

    public void SetRoomList(List<Map.Rect> _roomList)
    {
        roomList = _roomList;
    }

}
