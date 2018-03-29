using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectType { DOOR, UNBREAKABLE, BREAKABLE, CHAIR, ITEMBOX, VENDINMACHINE, LIGHT }

public class CustomObject : MonoBehaviour {

    public Vector3 position;
    public ObjectType objectType;
    public Sprite sprite;
    public bool isActive;

    public virtual void Init()
    {
        GetComponent<SpriteRenderer>().sprite = sprite;
        if(sprite)
            GetComponent<BoxCollider2D>().size = sprite.bounds.size;
        GetComponent<Light>().enabled = false;
    }

    public void SetPosition()
    {
        position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
    }

    public virtual void Active() { }
}

public class Door : CustomObject
{
    public override void Init()
    {
        base.Init();
        objectType = ObjectType.DOOR;
    }
    public override void Active()
    {
    }
}

public class UnbreakableBox : CustomObject
{
    public override void Init()
    {
        base.Init();
        objectType = ObjectType.UNBREAKABLE;
    }
    public override void Active()
    {
        Debug.Log("Unbreakalbe");
    }
}

public class BreakalbeBox : CustomObject
{
    public override void Init()
    {
        base.Init();
        objectType = ObjectType.BREAKABLE;
    }
    public override void Active()
    {
        Debug.Log("BreakalbeBox");
    }
}

public class VendingMachine : CustomObject
{
    public override void Init()
    {
        base.Init();
        objectType = ObjectType.VENDINMACHINE;
    }
    public override void Active()
    {
        Debug.Log("VendingMachine");
    }
}

public class Chair : CustomObject
{
    public override void Init()
    {
        base.Init();
        objectType = ObjectType.CHAIR;
    }
    public override void Active()
    {
        Debug.Log("Chair");
    }
}

public class ItemBox : CustomObject
{
    public override void Init()
    {
        base.Init();
        objectType = ObjectType.ITEMBOX;
    }
    public override void Active()
    {
        Debug.Log("ItemBox");
    }
}

public class LightObject : CustomObject
{
    public override void Init()
    {
        base.Init();
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y - 5f);
        objectType = ObjectType.LIGHT;
    }
    public override void Active()
    {
        Debug.Log("Light");
        GetComponent<Light>().enabled = !GetComponent<Light>().enabled;
    }
}