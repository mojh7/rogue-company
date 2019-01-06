using System.Collections.Generic;
using UnityEngine;

public class RoomSetManager : MonoBehaviourSingleton<RoomSetManager> {

    public Sprite[] doorSprites;

    [SerializeField]
    private FloorRoomSetGroup commonFloorSet;
    [SerializeField]
    private FloorRoomSetGroup[] floorRoomSet;
    List<RoomSetGroup> roomSetGroup;
    RoomSet zeroRoomset;

    public FloorRoomSetGroup[] FloorRoomSetGroups
    {
        get
        {
            return floorRoomSet;
        }
    }
    public void Init()
    {
        zeroRoomset = ScriptableObject.CreateInstance<RoomSet>();

        zeroRoomset.Init(0, 0, 3, 0, RoomType.NONE);
        roomSetGroup = new List<RoomSetGroup>();
        bool isExist;
        for (int i = 0; i < commonFloorSet.RoomSets.Length; i++)
        {
            isExist = false;
            for (int j = 0; j < roomSetGroup.Count; j++)
            {
                if (roomSetGroup[j].width == commonFloorSet.RoomSets[i].width && roomSetGroup[j].height == commonFloorSet.RoomSets[i].height)
                {
                    if (commonFloorSet.RoomSets[i].roomType == RoomType.MONSTER)
                        roomSetGroup[j].AddMonsterRoom(commonFloorSet.RoomSets[i]);
                    else
                        roomSetGroup[j].AddOtherRoom(commonFloorSet.RoomSets[i]);
                    isExist = true;
                }
            }
            if (!isExist)
            {
                roomSetGroup.Add(new RoomSetGroup(commonFloorSet.RoomSets[i].width, commonFloorSet.RoomSets[i].height));
                if (commonFloorSet.RoomSets[i].roomType == RoomType.MONSTER)
                    roomSetGroup[roomSetGroup.Count - 1].AddMonsterRoom(commonFloorSet.RoomSets[i]);
                else
                    roomSetGroup[roomSetGroup.Count - 1].AddOtherRoom(commonFloorSet.RoomSets[i]);
            }
        }
        for (int i = 0; i < commonFloorSet.HallSets.Length; i++)
        {
            isExist = false;
            for (int j = 0; j < roomSetGroup.Count; j++)
            {
                if (roomSetGroup[j].width == commonFloorSet.HallSets[i].width && roomSetGroup[j].height == commonFloorSet.HallSets[i].height)
                {
                    roomSetGroup[j].AddHallList(commonFloorSet.HallSets[i]);

                    isExist = true;
                }
            }
            if (!isExist)
            {
                roomSetGroup.Add(new RoomSetGroup(commonFloorSet.HallSets[i].width, commonFloorSet.HallSets[i].height));

                roomSetGroup[roomSetGroup.Count - 1].AddHallList(commonFloorSet.HallSets[i]);

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
