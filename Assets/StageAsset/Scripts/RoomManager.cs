using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Map;

public class RoomManager : MonoBehaviourSingleton<RoomManager> {

    List<Map.Rect> roomList;
    Map.Rect currentRoom;
    Transform mask;

    private int monsterNum = 0;
    Vector2 maskSize;
    Vector2 zeroVector;
    int mapSize;
    int clearedRoom;
    int KeyNum;
    int ammoNum;

    public Vector2 LeftPos
    {
        get
        {
            return new Vector2(currentRoom.areaLeftDown.x + 2.5f, (currentRoom.areaLeftDown.y + currentRoom.areaRightTop.y) * .5f);
        }
    }
    public Vector2 RightPos
    {
        get
        {
            return new Vector2(currentRoom.areaRightTop.x - 2.5f, (currentRoom.areaLeftDown.y + currentRoom.areaRightTop.y) * .5f);
        }
    }
    public Vector2 MidPos
    {
        get
        {
            return (currentRoom.areaLeftDown + currentRoom.areaRightTop) * .5f;
        }
    }
    public Vector2 TopPos
    {
        get
        {
            return new Vector2((currentRoom.areaLeftDown.x + currentRoom.areaRightTop.x) * .5f, currentRoom.areaRightTop.y - 2.5f);
        }
    }
    public Vector2 BottomPos
    {
        get
        {
            return new Vector2((currentRoom.areaLeftDown.x + currentRoom.areaRightTop.x) * .5f, currentRoom.areaLeftDown.y + 2.5f);
        }
    }

    protected Coroutine roomCoroutine;
    private void Awake()
    {
        if (mask == null)
        {
            mask = GetComponentInChildren<SpriteMask>(true).transform;
        }
    }

    public Vector3 MidPosition
    {
        get
        {
            return (currentRoom.areaLeftDown + currentRoom.areaRightTop) * .5f;
        }
    }

    public void Trap()
    {
        MiniMap.Instance.HideMiniMap();
        DoorClose();
        ObjectSetAvailable();
        DoorSetAvailable();
        currentRoom.gage = currentRoom.width * currentRoom.height * 3;
        currentRoom.isClear = false;
        SpawnMonster();
    }

    public bool isRoomClear()
    {
        if (currentRoom == null)
            return false;
        return currentRoom.isClear;
    }

    private void IniMask()
    {
        PlayerManager.Instance.GetPlayer().SetInFloor();
        mask.transform.localPosition = zeroVector;
        mask.transform.localScale = maskSize;
    }

    private void SetMask()
    {
        PlayerManager.Instance.GetPlayer().SetInRoom();

        mask.transform.localPosition = new Vector2(currentRoom.x * mapSize + currentRoom.width * mapSize * 0.5f + 0.5f, currentRoom.y * mapSize + currentRoom.height * mapSize * 0.5f);
        mask.transform.localScale = new Vector2(currentRoom.width * mapSize * 1.1f, currentRoom.height * mapSize * 1.1f + 1);
    }

    public void InitRoomList()
    {
        ammoNum = 2;
        KeyNum = Random.Range(1,3);
        clearedRoom = 0;
        roomList = MapManager.Instance.GetMap().GetList(out currentRoom);
        for (int i = 0; i < roomList.Count; i++)
        {
            if(roomList[i].eRoomType == RoomType.MONSTER)
            {
                clearedRoom++;
            }
        }
        mapSize = 3;
        maskSize = new Vector2(MapManager.Instance.currentMapSet.mapSize.x * mapSize + 0.5f, MapManager.Instance.currentMapSet.mapSize.y * mapSize + 1.5f);
        zeroVector = new Vector2(MapManager.Instance.currentMapSet.mapSize.x * mapSize * 0.5f + 0.5f, MapManager.Instance.currentMapSet.mapSize.y * mapSize * 0.5f - 0.5f);
        EconomySystem.Instance.InitFloorData(roomList);
        LockDoor();
    } // 룸리스트 받아오기

    void LockDoor()
    {
        if (GameStateManager.Instance.GetMode() == GameStateManager.GameMode.RUSH)
            return;
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].eRoomType == RoomType.EVENT || roomList[i].eRoomType == RoomType.BOSS)
            {
                roomList[i].DoorLock();
            }
        }

    }

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

    void DoorOpen()
    {
        FindCurrentRoom();

        if (currentRoom.doorObjects != null)
        {
            for (int j = 0; j < currentRoom.doorObjects.Count; j++)
            {
                currentRoom.doorObjects[j].Open();
            }
        }
    }

    void DoorClose()
    {
        StopCoroutine(roomCoroutine);
        roomCoroutine = null;

        if (currentRoom.doorObjects != null)
        {
            for (int j = 0; j < currentRoom.doorObjects.Count; j++)
            {
                currentRoom.doorObjects[j].Close();
            }
        }
    }

    void ObjectSetAvailable() 
    {
        if (currentRoom.customObjects != null)
        {
            for (int j = 0; j < currentRoom.customObjects.Length; j++)
            {
                if(currentRoom.customObjects[j].GetComponent<CustomObject>())
                    currentRoom.customObjects[j].GetComponent<CustomObject>().SetAvailable();
            }
        }
    } // 작동 가능여부 turn

    void DoorSetAvailable()
    {
        if (currentRoom.doorObjects != null)
        {
            for (int j = 0; j < currentRoom.doorObjects.Count; j++)
            {
                currentRoom.doorObjects[j].SetAvailable();
            }
        }
    }

    void EnableObjects()
    {
        if (!currentRoom.isRoom)
            return;
        if (currentRoom.customObjects == null)
            return;
        for (int i = 0; i < currentRoom.customObjects.Length; i++)
        {
            currentRoom.customObjects[i].SetActive(true);
        }
    }

    void DisalbeObject(Map.Rect _room)
    {
        if (_room.customObjects == null)
            return;
        for (int j = 0; j < _room.customObjects.Length; j++)
            _room.customObjects[j].SetActive(false);
    }

    void DropAmmo()
    {
        if (ammoNum <= 0 || clearedRoom <= 0)
            return;
        int eachKeyLow = ammoNum / clearedRoom;
        int eachKeyHigh = (ammoNum + clearedRoom - 1) / clearedRoom;

        int ret = Random.Range(eachKeyLow, eachKeyHigh + 1);
        if (ret == 1)
        {
            ammoNum--;
            ItemManager.Instance.DropAmmo(currentRoom.GetNearestAvailableArea(PlayerManager.Instance.GetPlayerPosition()));
        }
    }

    void DropKey()
    {
        if (KeyNum <= 0 || clearedRoom <= 0)
            return;
        int eachKeyLow = KeyNum / clearedRoom;
        int eachKeyHigh = (KeyNum + clearedRoom - 1) / clearedRoom;

        int ret = Random.Range(eachKeyLow, eachKeyHigh + 1);
        if (ret == 1)
        {
            KeyNum--;
            ItemManager.Instance.DropKey(currentRoom.GetAvailableArea());
        }
    }

    void ClearRoom()
    {
        SpawnManager.Instance.ResetProcess();

        DropKey(); DropAmmo();
        clearedRoom--;
        currentRoom.isClear = true;

        MiniMap.Instance.HideMiniMap();
        ObjectSetAvailable();
        DoorSetAvailable();
        DoorOpen();
        UnityContext.GetClock().RemoveAllTimer();
        UIManager.Instance.ClearRoomUI(true);
        UtilityClass.Invoke(this, ItemManager.Instance.CollectItem, 1f);

        if (currentRoom.eRoomType == RoomType.BOSS)
        {
            ItemManager.Instance.CallItemBox(currentRoom.GetNearestAvailableArea(PlayerManager.Instance.GetPlayerPosition()), RoomType.BOSS);
            ItemManager.Instance.DropKey(currentRoom.GetNearestAvailableArea(PlayerManager.Instance.GetPlayerPosition() + Vector3.right));
			CutSceneUI.Instance.ShowCutScene((GameDataManager.Instance.GetFloor() / 2) + 1);
        }
        for (int i = 0; i < currentRoom.customObjects.Length; i++)
        {
            if (currentRoom.customObjects[i].GetComponent<Portal>() != null)
            {
                currentRoom.customObjects[i].GetComponent<Portal>().Possible();
            }
        }
        NeignborDraw(currentRoom);
    }

    void SpawnMonster()
    {
        if (currentRoom.gage <= 0)
            return;
        SpawnManager.Instance.Spawn();
    } // 몬스터 소환

    public void SpawndWithGage(int gage)
    {
        currentRoom.gage -= gage;
        monsterNum++;
    }

    public void DieMonster()
    {
        monsterNum--;
        if (monsterNum == 0)
        {
            if (currentRoom.gage <= 0)
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
        if(roomCoroutine == null)
            roomCoroutine = StartCoroutine(FindRoomCoroutine());
    }

    IEnumerator FindRoomCoroutine() // 현재 방 찾기 코루틴
    {
        while (true)
        {
            yield return YieldInstructionCache.WaitForSeconds(0.1f);

            if (!currentRoom.isRoom || currentRoom.isClear)
            {
                currentRoom = GetCurrentRect(PlayerManager.Instance.GetPlayerPosition());
            }
            else
            {
                MapManager.Instance.GetMap().RemoveFog(currentRoom);
                SetMask();
                if (currentRoom.isLock)
                {
                    currentRoom.DoorUnLock();
                }
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
                    currentRoom.ShowDoorState();
                    currentRoom.isClear = true;
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
        MiniMap.Instance.ClearRoom(room, 1);

        for (int i = 0; i < room.linkedEdgeRect.Count; i++)
        {
            if(room.linkedEdgeRect[i].isRoom && !room.linkedEdgeRect[i].isDrawed &&
                !IsLinkedVerticalEvent(room,room.linkedEdgeRect[i]))
            {
                room.linkedEdgeRect[i].isDrawed = true;
                MiniMap.Instance.ClearRoom(room.linkedEdgeRect[i], 0.2f);
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
        EnableObjects();
        MiniMap.Instance.HideMiniMap();
        DoorClose();
        ObjectSetAvailable();
        DoorSetAvailable();
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
        CutSceneUI.Instance.SetCharacter(EnemyManager.Instance.GetBossSprite());
        CutSceneUI.Instance.BossCutScene(Vector2.right, Vector2.down, Vector2.right);
        CutSceneUI.Instance.SetText(EnemyManager.Instance.GetBossName());
    }

    void BossSceneEnd()
    {
        UIManager.Instance.TogglePreventObj();
        CutSceneUI.Instance.Hide();
        EnemyManager.Instance.SpawnBoss(GameDataManager.Instance.GetFloor());
        monsterNum++;
        currentRoom.gage--;
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
