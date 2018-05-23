using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectType { NONE, UNBREAKABLE, BREAKABLE, CHAIR, ITEMBOX, VENDINMACHINE, SPAWNER, PORTAL}

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
            List<Vector2> list = new List<Vector2>();
            sprite.GetPhysicsShape(0, list);
            GetComponent<PolygonCollider2D>().SetPath(0, list.ToArray());
            GetComponent<PolygonCollider2D>().isTrigger = false;
        }
        gameObject.tag = "Wall";
        gameObject.layer = 14;
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
        if (!isAvailable || !isAnimate)
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
    public override void SetAvailable()
    {
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
        GetComponent<PolygonCollider2D>().SetPath(0, null);
        objectType = ObjectType.SPAWNER;

        // 0516 모장현
        gameObject.tag = "Enemy";
        gameObject.layer = 13;
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
        isAvailable = false;
        GetComponent<PolygonCollider2D>().SetPath(0, null);
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
        isAvailable = true;
        objectType = ObjectType.PORTAL;
    }
    public override void SetAvailable()
    {
        this.gameObject.SetActive(false);
    }
    public override void Active()
    {
        base.Active();
        GamaManager.Instance.GoUpFloor();
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
    Item innerObject;

    public override void Init()
    {
        base.Init();
        GetComponent<PolygonCollider2D>().enabled = false;
        GetComponent<PolygonCollider2D>().isTrigger = true;
        isActive = false;
        isAvailable = true;
        objectType = ObjectType.NONE;
        gameObject.tag = "Untagged";
    }

    public void Init(Item _object)
    {
        Init();
        sprite = _object.GetComponent<SpriteRenderer>().sprite;
        innerObject = _object;
    }

    public override void Active()
    {
        base.Active();
        Debug.Log("ItemContainer");

        PlayerManager.Instance.GetPlayer().weaponManager.PickAndDropWeapon(innerObject, gameObject);
    }
}