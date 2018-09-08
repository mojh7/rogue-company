using System.Collections.Generic;
using UnityEngine;

public class RoomSetManager : MonoBehaviourSingleton<RoomSetManager> {

    public Sprite[] doorSprites;
    public RoomSet[] roomSetArr;
    public RoomSet[] hallSetArr;
    public FloorRoomSetGroup[] floorRoomSet;
    List<RoomSetGroup> roomSetGroup;
    RoomSet zeroRoomset;

    public void Init()
    {
        zeroRoomset = new RoomSet(0, 0, 3, 0, RoomType.NONE);

        roomSetGroup = new List<RoomSetGroup>();
        bool isExist;
        for (int i = 0; i < roomSetArr.Length; i++)
        {
            isExist = false;
            for (int j = 0; j < roomSetGroup.Count; j++)
            {
                if (roomSetGroup[j].width == roomSetArr[i].width && roomSetGroup[j].height == roomSetArr[i].height)
                {
                    if (roomSetArr[i].roomType == RoomType.MONSTER)
                        roomSetGroup[j].AddMonsterRoom(roomSetArr[i]);
                    else
                        roomSetGroup[j].AddOtherRoom(roomSetArr[i]);
                    isExist = true;
                }
            }
            if (!isExist)
            {
                roomSetGroup.Add(new RoomSetGroup(roomSetArr[i].width, roomSetArr[i].height));
                if (roomSetArr[i].roomType == RoomType.MONSTER)
                    roomSetGroup[roomSetGroup.Count - 1].AddMonsterRoom(roomSetArr[i]);
                else
                    roomSetGroup[roomSetGroup.Count - 1].AddOtherRoom(roomSetArr[i]);
            }
        }
        for (int i = 0; i < hallSetArr.Length; i++)
        {
            isExist = false;
            for (int j = 0; j < roomSetGroup.Count; j++)
            {
                if (roomSetGroup[j].width == hallSetArr[i].width && roomSetGroup[j].height == hallSetArr[i].height)
                {
                    roomSetGroup[j].AddHallList(hallSetArr[i]);

                    isExist = true;
                }
            }
            if (!isExist)
            {
                roomSetGroup.Add(new RoomSetGroup(hallSetArr[i].width, hallSetArr[i].height));

                roomSetGroup[roomSetGroup.Count - 1].AddHallList(hallSetArr[i]);

            }
        }
    }

    public RoomSet LoadRoomSet(int _width,int _height)
    {
        RoomSet roomSet = zeroRoomset;
        RoomSet temp = null;

        for (int i = 0;i< roomSetGroup.Count; i++)
        {
            if (_width == roomSetGroup[i].width && _height == roomSetGroup[i].height)
            {
                if (Random.Range(0, 10) >= 0)
                    roomSet = roomSetGroup[i].GetMonsterRoomSet();
                else
                    roomSet = roomSetGroup[i].GetOtherRoomSet();
                if (!roomSet)
                    continue;
                return roomSet;
            }
            else if (_width >= roomSetGroup[i].width && _height >= roomSetGroup[i].height)
            {
                if (Random.Range(0, 10) >= 0)
                    temp = roomSetGroup[i].GetMonsterRoomSet();
                else
                    temp = roomSetGroup[i].GetOtherRoomSet();
                if (temp == null)
                    continue;
                if (roomSet.width < temp.width && roomSet.height < temp.height)
                    roomSet = temp;
            }
        }

        if (temp == null)
        {
            for (int i = 0; i < roomSetGroup.Count; i++)
            {
                if (_width == roomSetGroup[i].width && _height == roomSetGroup[i].height)
                {
                    roomSet = roomSetGroup[i].GetOtherRoomSet();

                    if (!roomSet)
                        continue;
                    return roomSet;
                }
                else if (_width >= roomSetGroup[i].width && _height >= roomSetGroup[i].height)
                {
                    temp = roomSetGroup[i].GetOtherRoomSet();

                    if (temp == null)
                        continue;
                    if (roomSet.width < temp.width && roomSet.height < temp.height)
                        roomSet = temp;
                }
            }
        }

        return roomSet;  
    }

    public RoomSet LoadHallSet(int width,int height)
    {
        RoomSet roomSet = zeroRoomset;

        for (int i = 0; i < roomSetGroup.Count; i++)
        {
            if (width == roomSetGroup[i].width && height == roomSetGroup[i].height)
            {
                roomSet = roomSetGroup[i].GetHallSet();
                return roomSet;
            }
            else if(width >= roomSetGroup[i].width && height >= roomSetGroup[i].height)
            {
                RoomSet temp = roomSetGroup[i].GetHallSet();
                if (temp == null)
                    continue;
                if (roomSet.width < temp.width && roomSet.height < temp.height)
                    roomSet = temp;
            }
        }
        return roomSet;
    }
}
[CreateAssetMenu(menuName = "Room/RoomSetGroup")]
public class FloorRoomSetGroup : ScriptableObject
{
    [SerializeField]
    private RoomSet[] roomSets;
    [SerializeField]
    private RoomSet[] hallSets;
    [SerializeField]
    private ObjectSet[] objectSets;

    public RoomSet[] RoomSets
    {
        get
        {
            return roomSets;
        }
    }
    public RoomSet[] HallSets
    {
        get
        {
            return hallSets;
        }
    }
    public ObjectSet[] ObjectSets
    {
        get
        {
            return objectSets;
        }
    }
}

class RoomSetGroup
{
    public readonly int width;
    public readonly int height;
    public List<RoomSet> monsterList;
    public List<RoomSet> otherList;
    public List<RoomSet> hallList;

    public RoomSetGroup(int _width,int _height)
    {
        width = _width;
        height = _height;
        monsterList = new List<RoomSet>();
        otherList = new List<RoomSet>();
        hallList = new List<RoomSet>();
    }

    public void AddMonsterRoom(RoomSet _roomSet)
    {
        monsterList.Add(_roomSet);
    }
    public void AddOtherRoom(RoomSet _roomSet)
    {
        otherList.Add(_roomSet);
    }
    public void AddHallList(RoomSet roomSet)
    {
        hallList.Add(roomSet);
    }

    public RoomSet GetMonsterRoomSet()
    {
        if (monsterList.Count < 1)
            return null;
        return monsterList[Random.Range(0, monsterList.Count)];
    }
    public RoomSet GetOtherRoomSet()
    {
        if (otherList.Count < 1)
            return null;
        return otherList[Random.Range(0, otherList.Count)];
    }
    public RoomSet GetHallSet()
    {
        if (hallList.Count < 1)
            return null;
        return hallList[Random.Range(0, hallList.Count)];
    }
}
