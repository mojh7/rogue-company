using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Map;

public class RoomManager : MonoBehaviour {

    private static RoomManager instance;
    public GameObject maskPrefab;
    new public Transform transform;
    List<Map.Rect> roomList;
    Map.Rect currentRoom;

    public static RoomManager Getinstance()
    {
        if (instance == null)
        {
            instance = GameObject.FindObjectOfType(typeof(RoomManager)) as RoomManager;
        }
        return instance;
    }

    public void SetRoomList(List<Map.Rect> _roomList,Map.Rect _currentRoom)
    {
        roomList = _roomList;
        currentRoom = _currentRoom;
        StartCoroutine(FindCurrentRoom());
    } // 룸리스트 받아오기

    public void LoadMaskObject()
    {
        for(int i=0;i< roomList.Count; i++)
        {
            roomList[i].maskObject = Object.Instantiate(maskPrefab);
            roomList[i].LoadMaskObject();
            if (!roomList[i].isRoom)
                roomList[i].maskObject.SetActive(true);
            else
                roomList[i].maskObject.SetActive(true);
        }
    } // 마스크오브젝트 붙이기(수정해야함)

    public void DoorActive()
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].doorObjects != null)
            {
                for (int j = 0; j < roomList[i].doorObjects.Count; j++)
                {
                    roomList[i].doorObjects[j].GetComponent<CustomObject>().Active();
                }
            }
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
    } // 작동

    public List<Map.Rect> GetRoomList()
    {
        return roomList;
    }

    IEnumerator FindCurrentRoom() // 현재 방 찾기 코루틴
    {
        while (true)
        {
            if (!currentRoom.isClear)
            {

            }
            else
                currentRoom = GetCurrentRect(transform.position);
            yield return YieldInstructionCache.WaitForSeconds(0.1f);
        }
    }

    public Map.Rect GetCurrentRect(Vector3 _position) // 현재 방 찾기
    {
        for (int i = 0; i < currentRoom.edgeRect.Count; i++)
        {
            if (currentRoom.edgeRect[i].IsContain(_position))
            {
                return currentRoom.edgeRect[i];
            }
        }
        return currentRoom;
    }
}
