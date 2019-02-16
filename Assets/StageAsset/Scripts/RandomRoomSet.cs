using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Room/RandomRoomSet")]
public class RandomRoomSet : ScriptableObject
{
    public RoomSet roomSet;
    public float probability;
}
