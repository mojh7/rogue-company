using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSet : ScriptableObject
{
    int width;
    int height;
    public List<CustomObject> objects;

    public RoomSet(int _width, int _height)
    {
        width = _width;
        height = _height;
        objects = new List<CustomObject>();
    }

    public void Add(CustomObject _obj)
    {
        objects.Add(_obj);
    }
}