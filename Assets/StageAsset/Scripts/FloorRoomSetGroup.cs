using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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