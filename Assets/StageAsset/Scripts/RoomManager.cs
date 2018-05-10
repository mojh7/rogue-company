using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Map;

public class RoomManager : MonoBehaviourSingleton<RoomManager> {

    public Map.Rect tempRoom;
    List<Map.Rect> roomList;
    Map.Rect currentRoom;

    private int monsterNum = 0;

    public void InitRoomList()
    {
        roomList = MapManager.Instance.GetMap().GetList(out currentRoom);
    } // 룸리스트 받아오기

    void DoorSetAvailable() 
    {
        if (currentRoom.doorObjects != null)
        {
            for (int j = 0; j < currentRoom.doorObjects.Count; j++)
            {
                currentRoom.doorObjects[j].GetComponent<CustomObject>().SetAvailable();
            }
        }
    }//작동 가능여부 turn

    void ObjectSetAvailable() 
    {
        if (currentRoom.customObjects != null)
        {
            for (int j = 0; j < currentRoom.customObjects.Length; j++)
            {
                currentRoom.customObjects[j].GetComponent<CustomObject>().SetAvailable();
            }
        }
    } // 작동 가능여부 turn

    void EnableObjects()
    {
        if (!currentRoom.isRoom)
            return;
        for (int j = 0; j < currentRoom.customObjects.Length; j++)
            currentRoom.customObjects[j].SetActive(true);
    }

    void ClearRoom()
    {
        DoorSetAvailable();
        ObjectSetAvailable();
        FindCurrentRoom();
    }

    void SpawnMonster()
    {
        if (currentRoom.customObjects != null)
        {
            for (int j = 0; j < currentRoom.customObjects.Length; j++)
            {
                if (currentRoom.gage <= 0)
                    return;
                if (currentRoom.customObjects[j].GetComponent<Spawner>() != null)
                {
                    currentRoom.customObjects[j].GetComponent<Spawner>().Active();
                }
            }
        }
    } // 몬스터 소환

    public void DisableObjects()
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            if (!roomList[i].isRoom)
                continue;
            for (int j = 0; j < roomList[i].customObjects.Length; j++)
                roomList[i].customObjects[j].SetActive(false);
        }
    }

    public void Spawned()
    {
        currentRoom.gage--;
        monsterNum++;
    }

    public void DieMonster()
    {
        monsterNum--;
        if (monsterNum == 0)
        {
            if (currentRoom.gage == 0)
                ClearRoom();
            else
                SpawnMonster();
        }
    } // 몬스터 사망

    public List<Map.Rect> GetRoomList()
    {
        return roomList;
    } // 룸리스트

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
                currentRoom.isClear = true;
                EnableObjects();
                if (currentRoom.gage > 0)
                {
                    DoorSetAvailable();
                    ObjectSetAvailable();
                    SpawnMonster();
                    currentRoom.maskObject.SetActive(true);
                    MiniMap.Instance.DrawRoom(currentRoom);
                    break;
                }
                MiniMap.Instance.DrawRoom(currentRoom);
            }
            else
                currentRoom = GetCurrentRect(PlayerManager.Instance.GetPlayerPosition());
            currentRoom.maskObject.SetActive(true);

            yield return YieldInstructionCache.WaitForSeconds(0.05f);
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
