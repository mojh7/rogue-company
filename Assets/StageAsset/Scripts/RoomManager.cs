using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Map;

public class RoomManager : MonoBehaviour {

    private static RoomManager instance;
    public GameObject maskObject;
    public GameObject maskPrefab;
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

    public void LoadMaskObject()
    {
        for(int i=0;i< roomList.Count; i++)
        {
            roomList[i].maskObject = Object.Instantiate(maskPrefab);
            roomList[i].maskObject.transform.parent = maskObject.transform;
            roomList[i].LoadMaskObject();
            if (!roomList[i].isRoom)
                roomList[i].maskObject.SetActive(true);
            else
                roomList[i].maskObject.SetActive(true);
        }
    }

    public void Active()
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].customObjects != null)
            {
                for (int j = 0; j < roomList[i].customObjects.Length; j++)
                {
                    roomList[i].customObjects[j].GetComponent<CustomObject>().Active();
                }
            }
        }
    }

    public List<Map.Rect> GetRoomList()
    {
        return roomList;
    }

    //public Map.Rect GetCurrentRect()
    //{

    //}
}
