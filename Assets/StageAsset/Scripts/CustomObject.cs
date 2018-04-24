using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectType { DOOR, UNBREAKABLE, BREAKABLE, CHAIR, ITEMBOX, VENDINMACHINE, SPAWNER, START, END }

public class CustomObject : MonoBehaviour {

    public Vector3 position;
    public ObjectType objectType;
    public Sprite sprite;
    public Sprite[] sprites;
    public bool isActive;
    public bool isAvailable;
    protected bool isAnimate;
    protected SpriteRenderer spriteRenderer;
    protected Animator animator;
    protected BoxCollider2D boxCollider;

    public virtual void Init()
    {
        isAnimate = false;
        if (sprites != null)
            sprite = sprites[Random.Range(0, sprites.Length)];
        if (sprite)
        {
            GetComponent<SpriteRenderer>().sprite = sprite;
            GetComponent<BoxCollider2D>().size = sprite.bounds.size;
        }
    }

    public void SetPosition()
    {
        position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
    }

    public void SetAvailable()
    {
        isAvailable = !isAvailable;
    }

    public void SetAnimate()
    {
        isAnimate = false;
    }

    public virtual void Active()
    {
        if (!isAvailable)
            return;
    }

#region UnityFunc
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void LateUpdate()
    {
        if (!isAnimate)
            spriteRenderer.sprite = sprite;
    }
#endregion
}

public class Door : CustomObject
{
    bool isHorizon;

    public override void Init()
    {
        base.Init();
        isActive = true;
        isAvailable = true;
        objectType = ObjectType.DOOR;
    }
    public override void Active()
    {
        base.Active();
        isAnimate = true;
        if(!isHorizon)
            animator.SetTrigger("door_horizon");
        else
            animator.SetTrigger("door_vertical");
        Debug.Log("Door");
    }
    
    public void SetAxis(bool _isHorizon)
    {
        isHorizon = _isHorizon;
    }
}

public class UnbreakableBox : CustomObject
{
    public override void Init()
    {
        base.Init();
        isActive = false;
        isAvailable = false;
        objectType = ObjectType.UNBREAKABLE;
    }
    public override void Active()
    {
        base.Active();
        Debug.Log("Unbreakalbe");
    }
}

public class BreakalbeBox : CustomObject
{
    public override void Init()
    {
        base.Init();
        isActive = false;
        isAvailable = false;
        objectType = ObjectType.BREAKABLE;
    }
    public override void Active()
    {
        base.Active();
        Debug.Log("BreakalbeBox");
    }
}

public class VendingMachine : CustomObject
{
    public override void Init()
    {
        base.Init();
        isActive = false;
        isAvailable = false;
        objectType = ObjectType.VENDINMACHINE;
    }
    public override void Active()
    {
        base.Active();
        Debug.Log("VendingMachine");
    }
}

public class Chair : CustomObject
{
    public override void Init()
    {
        base.Init();
        isActive = false;
        isAvailable = false;
        objectType = ObjectType.CHAIR;
    }
    public override void Active()
    {
        base.Active();
        Debug.Log("Chair");
    }
}

public class ItemBox : CustomObject
{
    public override void Init()
    {
        base.Init();
        isActive = false;
        isAvailable = false;
        objectType = ObjectType.ITEMBOX;
    }
    public override void Active()
    {
        base.Active();
        Debug.Log("ItemBox");
    }
}

public class Spawner : CustomObject
{
    public override void Init()
    {
        base.Init();
        isActive = false;
        isAvailable = false;
        GetComponent<BoxCollider2D>().size = new Vector2(0, 0);
        objectType = ObjectType.SPAWNER;
    }
    public override void Active()
    {
        base.Active();
        EnemyGenerator.GetInstance().Generate(transform.position);
        Debug.Log("Spawner");
    }
}

public class StartPoint : CustomObject
{
    public override void Init()
    {
        base.Init();
        isActive = false;
        isAvailable = true;
        GetComponent<BoxCollider2D>().size = new Vector2(0, 0);
        objectType = ObjectType.START;
    }
    public override void Active()
    {
        base.Active();
        Debug.Log("PlayerStart");
    }
}

public class EndPoint : CustomObject
{
    public override void Init()
    {
        base.Init();
        isActive = false;
        isAvailable = false;
        GetComponent<BoxCollider2D>().size = new Vector2(0,0);
        objectType = ObjectType.END;
    }
    public override void Active()
    {
        base.Active();
        Debug.Log("PlayerEnd");
    }
}
