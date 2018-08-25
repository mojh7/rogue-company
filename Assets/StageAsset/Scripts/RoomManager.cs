using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Map;

public class RoomManager : MonoBehaviourSingleton<RoomManager> {

    List<Map.Rect> roomList;
    Map.Rect currentRoom;
    [SerializeField]
    Transform mask;

    private int monsterNum = 0;
    Vector2 maskSize;
    Vector2 zeroVector;
    int mapSize;
    
    private void IniMask()
    {
        PlayerManager.Instance.GetPlayer().SetInFloor();

        mask.transform.localPosition = zeroVector;
        mask.transform.localScale = maskSize;
    }

    private void SetMask()
    {
        PlayerManager.Instance.GetPlayer().SetInRoom();

        mask.transform.localPosition = new Vector2(currentRoom.x * mapSize + currentRoom.width * mapSize * 0.5f + 0.5f, currentRoom.y * mapSize + currentRoom.height * mapSize * 0.5f - 0.5f);
        mask.transform.localScale = new Vector2(currentRoom.width * mapSize * 1.1f, currentRoom.height * mapSize * 1.1f + 1);
    }

    public void InitRoomList()
    {
        roomList = MapManager.Instance.GetMap().GetList(out currentRoom);
        for (int i = 0; i < roomList.Count; i++)
        {
            if(roomList[i].eRoomType != RoomType.BOSS && roomList[i].eRoomType != RoomType.MONSTER)
            {
                DisalbeObject(roomList[i]);
            }
        }
        mapSize = MapManager.Instance.size;
        maskSize = new Vector2(MapManager.Instance.width * mapSize + 0.5f, MapManager.Instance.height * mapSize + 1.5f);
        zeroVector = new Vector2(MapManager.Instance.width * mapSize * 0.5f + 0.5f, MapManager.Instance.height * mapSize * 0.5f - 0.5f);
    } // 룸리스트 받아오기

    public int GetGage()
    {
        return currentRoom.gage;
    }

    public Vector2 GetCurrentRoomAvailableArea()
    {
        return currentRoom.GetAvailableArea();
    }

    public Vector2 GetNearestAvailableArea(Vector2 position)
    {
        return currentRoom.GetNearestAvailableArea(position);
    }

    public void GetCurrentRoomBound(out Vector3 leftDown,out Vector3 rightTop)
    {
        leftDown = currentRoom.areaLeftDown;
        rightTop = currentRoom.areaRightTop;
    }

    void DoorActive() 
    {
        if (currentRoom.doorObjects != null)
        {
            for (int j = 0; j < currentRoom.doorObjects.Count; j++)
            {
                currentRoom.doorObjects[j].GetComponent<CustomObject>().Active();
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
        for (int i = 0; i < currentRoom.customObjects.Length; i++)
        {
            if (currentRoom.customObjects[i].GetComponent<Portal>() == null)
            {
                currentRoom.customObjects[i].SetActive(true);
            }
        }
    }

    void DisalbeObject(Map.Rect _room)
    {
        if (_room.customObjects == null)
            return;
        for (int j = 0; j < _room.customObjects.Length; j++)
            _room.customObjects[j].SetActive(false);
    }

    void ClearRoom()
    {
        MiniMap.Instance.HideMiniMap();
        DoorActive();
        ObjectSetAvailable();
        FindCurrentRoom();
        UnityContext.GetClock().RemoveAllTimer();
        Item item = ObjectPoolManager.Instance.CreateUsableItem(UsableItemType.CLOTHING);
        ItemManager.Instance.CallItemBox(currentRoom.GetNearestAvailableArea(PlayerManager.Instance.GetPlayerPosition() + Random.onUnitSphere), item);
        ItemManager.Instance.CollectItem();
        if (currentRoom.eRoomType == RoomType.BOSS)
        {
            for (int i = 0; i < currentRoom.customObjects.Length; i++)
            {
                if (currentRoom.customObjects[i].GetComponent<Portal>() != null)
                {
                    currentRoom.customObjects[i].SetActive(true);
                }
            }
        }
        NeignborDraw(currentRoom);
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
                    break;
                }
            }
        }
    } // 몬스터 소환

    public Vector3 Spawned()
    {
        monsterNum++;
        return currentRoom.GetAvailableArea();
    }

    public Vector3 SpawndWithGage()
    {
        currentRoom.gage--;
        monsterNum++;
        return currentRoom.GetAvailableArea();
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
        return MapManager.Instance.GetMap().GetStartPosition();
    }

    public void FindCurrentRoom()
    {
        StartCoroutine("FindRoomCoroutine");
    }

    IEnumerator FindRoomCoroutine() // 현재 방 찾기 코루틴
    {
        while (true)
        {
            yield return YieldInstructionCache.WaitForSeconds(0.05f);

            if (!currentRoom.isRoom || currentRoom.isClear)
            {
                currentRoom = GetCurrentRect(PlayerManager.Instance.GetPlayerPosition());
            }
            else
            {
                MapManager.Instance.GetMap().RemoveFog(currentRoom);
                SetMask();
                currentRoom.isClear = true;
                if (currentRoom.eRoomType == RoomType.BOSS) //보스 방
                {
                    InitRoom();
                    InitBossRoom();
                    break;
                }
                else if (currentRoom.eRoomType == RoomType.MONSTER)
                {
                    InitRoom();
                    SpawnMonster();
                    break;
                }
                else
                {
                    NeignborDraw(currentRoom);
                    EnableObjects();
                    ObjectSetAvailable();
                }
            }
        }
    }

    void NeignborDraw(Map.Rect room)
    {
        if (!room.isRoom)
            return;
        room.isDrawed = true;
        MiniMap.Instance.ClearRoom(room);

        for (int i = 0; i < room.linkedEdgeRect.Count; i++)
        {
            if(room.linkedEdgeRect[i].isRoom && !room.linkedEdgeRect[i].isDrawed &&
                !IsLinkedVerticalEvent(room,room.linkedEdgeRect[i]))
            {
                room.linkedEdgeRect[i].isDrawed = true;
                MiniMap.Instance.ClearRoom(room.linkedEdgeRect[i]);
            }
        }
    }

    bool IsLinkedVerticalEvent(Map.Rect _rectA, Map.Rect _rectB) // 두개의 방을 직접 연결
    {
        if ((Mathf.Abs(_rectA.midX - _rectB.midX) < (float)(_rectA.width + _rectB.width) / 2) &&
            (Mathf.Abs(_rectA.midY - _rectB.midY) == (float)(_rectA.height + _rectB.height) / 2)) // 세로로 연결된 방
        {
            if (_rectA.midY > _rectB.midY)
            {
                if (_rectB.eRoomType == RoomType.REST || _rectB.eRoomType == RoomType.STORE)
                    return true;
            }
            else
            {
                if (_rectA.eRoomType == RoomType.REST || _rectA.eRoomType == RoomType.STORE)
                    return true;
            }
        }
        return false;
    }

    void InitRoom()
    {
        MiniMap.Instance.HideMiniMap();
        DoorActive();
        ObjectSetAvailable();
    }

    void InitBossRoom()
    {
        StartCoroutine(CoroutineBoss());
    }

    IEnumerator CoroutineBoss()
    {
        BossInitScene();
        yield return YieldInstructionCache.WaitForSeconds(2);
        BossSceneEnd();
    }

    void BossInitScene()
    {
        UIManager.Instance.TogglePreventObj();
        EnemyManager.Instance.CreateBossData();
        CutSceneUI.Instance.SetText(EnemyManager.Instance.GetBossName());
        CutSceneUI.Instance.SetCharacter(EnemyManager.Instance.GetBossSprite());
        CutSceneUI.Instance.ShowCutScene(Vector2.right, Vector2.down, Vector2.up);
    }

    void BossSceneEnd()
    {
        UIManager.Instance.TogglePreventObj();
        CutSceneUI.Instance.Hide();
        EnemyManager.Instance.SpawnBoss(GameDataManager.Instance.GetFloor(), (currentRoom.areaLeftDown + currentRoom.areaRightTop) / 2);
        monsterNum++;
        SpawnMonster();
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
        if (currentRoom.isRoom)
            SetMask();
        else
            IniMask();
        return currentRoom;
    }

}
