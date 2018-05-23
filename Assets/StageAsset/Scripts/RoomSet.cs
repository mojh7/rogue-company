using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType { NONE, MONSTER, EVENT, BOSS, STORE }

public class RoomSet : ScriptableObject
{
    public int x;
    public int y;
    public int width;
    public int height;
    public int size;
    public int gage;
    public List<ObjectData> objectDatas;
    public RoomType roomType;

    public RoomSet(int _width, int _height,int _size,int _gage, RoomType _roomType)
    {
        width = _width;
        height = _height;
        size = _size;
        gage = _gage;
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
    public Sprite[] sprites;
    public bool isActive;
    public ObjectType objectType;

    public ObjectData(Vector3 _position, ObjectType _objectType, Sprite[] _sprites)
    {
        position = _position;
        sprites = _sprites;
        objectType = _objectType;
        isActive = false;
    }

    public GameObject LoadObject(GameObject _gameObject)
    {
        switch (objectType)
        {
            case ObjectType.UNBREAKABLE:
                _gameObject.AddComponent<UnbreakableBox>();
                _gameObject.GetComponent<UnbreakableBox>().sprites = sprites;
                _gameObject.GetComponent<UnbreakableBox>().Init();
                break;
            case ObjectType.BREAKABLE:
                _gameObject.AddComponent<BreakalbeBox>();
                _gameObject.GetComponent<BreakalbeBox>().sprites = sprites;
                _gameObject.GetComponent<BreakalbeBox>().Init();
                break;
            case ObjectType.CHAIR:
                _gameObject.AddComponent<Chair>();
                _gameObject.GetComponent<Chair>().sprites = sprites;
                _gameObject.GetComponent<Chair>().Init();
                break;
            case ObjectType.ITEMBOX:
                _gameObject.AddComponent<ItemBox>();
                _gameObject.GetComponent<ItemBox>().sprites = sprites;
                _gameObject.GetComponent<ItemBox>().Init();
                break;
            case ObjectType.VENDINMACHINE:
                _gameObject.AddComponent<VendingMachine>();
                _gameObject.GetComponent<VendingMachine>().sprites = sprites;
                _gameObject.GetComponent<VendingMachine>().Init();
                break;
            case ObjectType.SPAWNER:
                _gameObject.AddComponent<Spawner>();
                _gameObject.GetComponent<Spawner>().sprite = null;
                _gameObject.GetComponent<Spawner>().Init();
                break;
            case ObjectType.PORTAL:
                _gameObject.AddComponent<Portal>();
                _gameObject.GetComponent<Portal>().sprites = sprites;
                _gameObject.GetComponent<Portal>().Init();
                break;
        }

        return _gameObject;
    }
}