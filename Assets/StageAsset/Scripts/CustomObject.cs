using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectType { MOVED, EVENT, BLOCK }

public class CustomObject : MonoBehaviour {

    public readonly float x;
    public readonly float y;
    public Sprite sprite;

    public void Init(Sprite _sprite)
    {
        sprite = _sprite;
        SetObject();
    }

    public void SetObject()
    {
        GetComponent<SpriteRenderer>().sprite = sprite;
    }

    virtual protected void Active() { }
}

public class WheelChair : CustomObject
{

}

public class NoWheelChair : CustomObject
{

}

public class VendingMachine : CustomObject
{

}

public class Block : CustomObject
{

}


