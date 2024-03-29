﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Room/RoomSetGroup")]
public class FloorRoomSetGroup : ScriptableObject
{
    [SerializeField]
    Map.MapSetting mapSetting;
    [SerializeField]
    private RandomTile _floorTile = null;

    [SerializeField]
    private string _floorName = null;

    public string floorName
    {
        get
        {
            return _floorName;
        }
    }
    [SerializeField]
    private RoomSet[] roomSets;
    [SerializeField]
    private RoomSet[] hallSets;
    [SerializeField]
    private ObjectSet[] objectSets;
    [SerializeField]
    private RandomRoomSet[] randomSets;   

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

    public RoomSet RandomSets
    {
        get
        {
            float total = 0;
            for (int i = 0; i < randomSets.Length; i++)
            {
                total += randomSets[i].probability;
            }

            Random.InitState((int)System.DateTime.Now.Ticks);

            float randomPoint = Random.value * total;


            for (int i = 0; i < randomSets.Length; i++)
            {
                total += randomSets[i].probability;

                if (randomPoint < randomSets[i].probability)
                {
                    return randomSets[i].roomSet;
                }
                else
                {
                    randomPoint -= randomSets[i].probability;
                }
            }
         
            return null;
        }
    }
    public RandomTile FloorTile
    {
        get
        {
            return _floorTile;
        }
    }
    public Map.MapSetting MapSetting
    {
        get
        {
            return mapSetting;
        }
    }
}