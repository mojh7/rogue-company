using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType { MONSTER, EVENT, BOSS, STORE }

public class RoomSet : ScriptableObject
{
    public int x;
    public int y;
    public int width;
    public int height;
    public int size;
    public List<ObjectData> objectDatas;
    public RoomType roomType;

    public RoomSet(int _width, int _height,int _size, RoomType _roomType)
    {
        width = _width;
        height = _height;
        size = _size;
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

    public GameObject LoadObject(GameObject _gameObject)
    {
        switch (objectType)
        {
            case ObjectType.UNBREAKABLE:
                _gameObject.AddComponent<UnbreakableBox>();
                _gameObject.GetComponent<UnbreakableBox>().sprite = sprite;
                _gameObject.GetComponent<UnbreakableBox>().Init();
                break;
            case ObjectType.BREAKABLE:
                _gameObject.AddComponent<BreakalbeBox>();
                _gameObject.GetComponent<BreakalbeBox>().sprite = sprite;
                _gameObject.GetComponent<BreakalbeBox>().Init();
                break;
            case ObjectType.CHAIR:
                _gameObject.AddComponent<Chair>();
                _gameObject.GetComponent<Chair>().sprite = sprite;
                _gameObject.GetComponent<Chair>().Init();
                break;
            case ObjectType.ITEMBOX:
                _gameObject.AddComponent<ItemBox>();
                _gameObject.GetComponent<ItemBox>().sprite = sprite;
                _gameObject.GetComponent<ItemBox>().Init();
                break;
            case ObjectType.VENDINMACHINE:
                _gameObject.AddComponent<VendingMachine>();
                _gameObject.GetComponent<VendingMachine>().sprite = sprite;
                _gameObject.GetComponent<VendingMachine>().Init();
                break;
            case ObjectType.DOOR:
                _gameObject.AddComponent<Door>();
                _gameObject.GetComponent<Door>().sprite = null;
                _gameObject.GetComponent<Door>().Init();
                break;
        }

        return _gameObject;
    }
}