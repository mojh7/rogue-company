using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSet : ScriptableObject
{
    public ObjectData objectData;
    public void Add(ObjectData _obj)
    {
        objectData = _obj;
    }

}