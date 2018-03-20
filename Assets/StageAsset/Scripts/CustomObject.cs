using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectType { UNBREAKABLE, BREAKABLE, CHAIR, ITEMBOX, VENDINMACHINE }

public class CustomObject : MonoBehaviour {

    public Vector3 position;
    public ObjectType objectType;
    public Sprite sprite;
    public bool isActive;

    public virtual void Init()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
    }

    public void SetPosition()
    {
        position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
    }

    public void Dispose()
    {
        Destroy(this);
    }

    public virtual void Active() { }
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


