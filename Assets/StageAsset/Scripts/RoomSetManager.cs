using System.Collections.Generic;
using UnityEngine;

public class RoomSetManager : MonoBehaviourSingleton<RoomSetManager> {

    public Sprite[] doorSprites;
    public RoomSet[] roomSetArr;
    public RoomSet[] firstFloorSet;
    List<RoomSetGroup> roomSetGroup;

    public void Init()
    {
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
    }

    public RoomSet LoadRoomSet(int _width,int _height,int floor)
    {
        RoomSet roomSet;

        for (int i = 0;i< roomSetGroup.Count; i++)
        {
            if (_width == roomSetGroup[i].width && _height == roomSetGroup[i].height)
            {
                if (Random.Range(0, 10) >= 1)
                    roomSet = roomSetGroup[i].GetMonsterRoomSet();
                else
                    roomSet = roomSetGroup[i].GetOtherRoomSet();
                return roomSet;
            }
        }
        return roomSetGroup[0].GetMonsterRoomSet(); ;
    }
}

class RoomSetGroup
{
    public readonly int width;
    public readonly int height;
    public List<RoomSet> monsterList;
    public List<RoomSet> otherList;

    public RoomSetGroup(int _width,int _height)
    {
        width = _width;
        height = _height;
        monsterList = new List<RoomSet>();
        otherList = new List<RoomSet>();
    }

    public void AddMonsterRoom(RoomSet _roomSet)
    {
        monsterList.Add(_roomSet);
    }
    public void AddOtherRoom(RoomSet _roomSet)
    {
        otherList.Add(_roomSet);
    }

    public RoomSet GetMonsterRoomSet()
    {
        return monsterList[Random.Range(0, monsterList.Count)];
    }
    public RoomSet GetOtherRoomSet()
    {
        return otherList[Random.Range(0, otherList.Count)];
    }
}
