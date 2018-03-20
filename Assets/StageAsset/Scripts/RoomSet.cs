using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType { MONSTER, EVENT, BOSS, STORE }

public class RoomSet : ScriptableObject
{
    public int width;
    public int height;
    public List<ObjectData> objectDatas;
    public RoomType roomType;

    public RoomSet(int _width, int _height,RoomType _roomType)
    {
        width = _width;
        height = _height;
        roomType = _roomType;
        objectDatas = new List<ObjectData>();
    }

    public void Add(ObjectData _obj)
    {
        objectDatas.Add(_obj);
    }
}

[System.Serializable]
public struct ObjectData
{
    public Vector3 position;
    public Sprite sprite;
    public bool isActive;
    public ObjectType objectType;
    public ObjectData(Vector3 _position,Sprite _sprite,ObjectType _objectType)
    {
        position = _position;
        sprite = _sprite;
        objectType = _objectType;
        isActive = false;
    }
}