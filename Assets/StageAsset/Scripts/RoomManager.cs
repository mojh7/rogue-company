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

    public void DoorSetAvailable() //작동 가능여부 turn
    {
        if (currentRoom.doorObjects != null)
        {
            for (int j = 0; j < currentRoom.doorObjects.Count; j++)
            {
                currentRoom.doorObjects[j].GetComponent<CustomObject>().SetAvailable();
            }
        }
    }

    public void ObjectSetAvailable() // 작동 가능여부 turn
    {
        if (currentRoom.customObjects != null)
        {
            for (int j = 0; j < currentRoom.customObjects.Length; j++)
            {
                currentRoom.customObjects[j].GetComponent<CustomObject>().SetAvailable();
            }
        }
    } // 작동 가능여부 turn

    public List<Map.Rect> GetRoomList()
    {
        return roomList;
    }

    public Vector3 RoomStartPoint()
    {
        for(int i = 0; i < currentRoom.customObjects.Length; i++)
        {
            if (currentRoom.customObjects[i].GetComponent<StartPoint>() != null)
                return currentRoom.customObjects[i].GetComponent<Transform>().position;
        }
        return Vector3.zero;
    }

    public void FindCurrentRoom()
    {
        StartCoroutine("FindRoomCoroutine");
    }

    IEnumerator FindRoomCoroutine() // 현재 방 찾기 코루틴
    {
        while (true)
        {
            if (!currentRoom.isClear)
            {
                DoorSetAvailable();
                ObjectSetAvailable();
                break;
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
