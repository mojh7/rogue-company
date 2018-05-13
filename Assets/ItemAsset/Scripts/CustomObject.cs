using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectType { NONE, UNBREAKABLE, BREAKABLE, CHAIR, ITEMBOX, VENDINMACHINE, SPAWNER}

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
            GetComponent<BoxCollider2D>().isTrigger = false;
        }
        gameObject.tag = "Wall";
    }

    public void SetPosition()
    {
        position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
    }

    public virtual void SetAvailable()
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

public class Spawner : CustomObject
{
    int spawnCount = 2;
    int gage;
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
        StartCoroutine(SpawnProcess());
        Debug.Log("Spawner");
    }
    IEnumerator SpawnProcess()
    {
        if (spawnCount >= 2)
        {
            gage = RoomManager.Instance.GetGage();
            int count = gage / 2;
            while (count > 0)
            {
                Spawn();
                count--;
                yield return YieldInstructionCache.WaitForSeconds(.5f);
            }

            spawnCount--;
        }
        else if (spawnCount == 1)
        {
            while (gage > 0)
            {
                Spawn();
                yield return YieldInstructionCache.WaitForSeconds(.5f);
            }

            spawnCount--;
        }
    }
    void Spawn()
    {
        gage--;
        Vector2 tempPosition = RoomManager.Instance.Spawned();
        EnemyGenerator.Instance.Generate(tempPosition);
    }
}

public class Door : CustomObject
{
    bool isHorizon;

    public override void Init()
    {
        base.Init();
        isActive = true;
        isAvailable = true;
        objectType = ObjectType.NONE;
        tag = "Wall";
    }
    public override void Active()
    {
        base.Active();
        isAnimate = true;
        if (!isHorizon)
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

public class Alert : CustomObject
{
    public delegate void Del(Vector3 _position);
    Del callback;
    public override void Init()
    {
        base.Init();
        isActive = false;
        isAvailable = true;
        objectType = ObjectType.NONE;
    }
    public void Init(Del _call)
    {
        Init();
        callback += _call;
    }
    public override void Active()
    {
        base.Active();
        Debug.Log("Alert");
        isAnimate = true;
        animator.SetTrigger("alert_indicator");
        StartCoroutine(CheckAnimate());
    }
    IEnumerator CheckAnimate()
    {
        while (true)
        {
            if (!isAnimate)
                break;
            yield return YieldInstructionCache.WaitForEndOfFrame;
        }

        callback(transform.position);
        Destroy(this.gameObject);
    }
}

public class Portal : CustomObject
{
    public override void Init()
    {
        base.Init();
        isActive = false;
        isAvailable = false;
        GetComponent<BoxCollider2D>().size = new Vector2(0,0);
        objectType = ObjectType.NONE;
    }
    public override void Active()
    {
        base.Active();
        Debug.Log("PlayerEnd");
    }
}

public class ItemBox : CustomObject
{
    public override void Init()
    {
        base.Init();
        isActive = false;
        isAvailable = true;
        objectType = ObjectType.ITEMBOX;
    }
    public override void Active()
    {
        base.Active();
        isAvailable = false;
        Debug.Log("ItemBox");
        ItemManager.Instance.DropItem(this.transform.position);
        Destroy(this.gameObject, 3);
    }
}

public class ItemContainer : CustomObject
{
    public override void Init()
    {
        base.Init();
        GetComponent<BoxCollider2D>().isTrigger = true;
        isActive = false;
        isAvailable = true;
        objectType = ObjectType.NONE;
    }

    public override void Active()
    {
        base.Active();
        Debug.Log("ItemContainer");
    }
}