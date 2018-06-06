using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectType { NONE, UNBREAKABLE, BREAKABLE, PUSHBOX, ITEMBOX, VENDINMACHINE, SPAWNER, PORTAL}

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
    protected Rigidbody2D rigidbody2D;

    public virtual void Init()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.bodyType = RigidbodyType2D.Static;
        isAnimate = false;
        if (sprites != null)
            sprite = sprites[Random.Range(0, sprites.Length)];
        if (sprite)
        {
            GetComponent<SpriteRenderer>().sprite = sprite;
            List<Vector2> list = new List<Vector2>();
            int num = sprite.GetPhysicsShapeCount();
            GetComponent<PolygonCollider2D>().pathCount = num;
            for (int i = 0; i < num; i++)
            {
                sprite.GetPhysicsShape(i, list);
                GetComponent<PolygonCollider2D>().SetPath(i, list.ToArray());
            }
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
        isActive = true;
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
    //public override void Active()
    //{
    //    base.Active();
    //    Debug.Log("Unbreakalbe");
    //}
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
    //public override void Active()
    //{
    //    base.Active();
    //    Debug.Log("BreakalbeBox");
    //}
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

public class PushBox : CustomObject
{
    public override void Init()
    {
        base.Init();
        rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        isActive = false;
        isAvailable = true;
        objectType = ObjectType.PUSHBOX;
    }
    public override void Active()
    {
        base.Active();
        Vector2 dir = transform.position - PlayerManager.Instance.GetPlayerPosition();
        
        rigidbody2D.AddForce(dir*1000);
        StartCoroutine(CoroutinePushed());
    }

    IEnumerator CoroutinePushed()
    {
        while (isActive)
        {
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
            if (rigidbody2D.velocity.magnitude < 1f)
            {
                rigidbody2D.velocity = Vector2.zero;
                isActive = false;
            }
        }
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
        gameObject.layer = LayerMask.NameToLayer("Enemy");
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
        EnemyManager.Instance.Generate(tempPosition);
    }
}

public class Door : CustomObject
{
    bool isHorizon;
    Sprite openSprite;
    Sprite closeSprite;

    public override void Init()
    {
        base.Init();
        objectType = ObjectType.NONE;
        tag = "Wall";
    }
    public void Init(Sprite _openSprite,Sprite _closeSprite)
    {
        Init();
        openSprite = _openSprite;
        closeSprite = _closeSprite;
        sprite = openSprite;
        SetCollision();
    }
    void SetCollision()
    {
        GetComponent<SpriteRenderer>().sprite = sprite;
        List<Vector2> list = new List<Vector2>();
        int num = sprite.GetPhysicsShapeCount();
        GetComponent<PolygonCollider2D>().pathCount = num;
        for (int i = 0; i < num; i++)
        {
            sprite.GetPhysicsShape(i, list);
            GetComponent<PolygonCollider2D>().SetPath(i, list.ToArray());
        }
        GetComponent<PolygonCollider2D>().isTrigger = false;
    }
    public override void Active()
    {
        base.Active();
        if (isActive)
        {
            isActive = false;
            sprite = openSprite;
        }
        else
        {
            isActive = true;
            isAnimate = true;
            if (!isHorizon)
            {
                animator.SetTrigger("door_horizon");
            }
            else
            {
                animator.SetTrigger("door_vertical");
            }
            sprite = closeSprite;
        }
       
        SetCollision();
    }
    public void SetAxis(bool _isHorizon)
    {
        isHorizon = _isHorizon;
    }
    public bool GetHorizon() { return isHorizon; }
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
        isAvailable = false;
        InGameManager.Instance.GoUpFloor();
        Debug.Log("PlayerEnd");
    }
}

public class ItemBox : CustomObject
{
    Item item;

    public override void Init()
    {
        base.Init();
        isActive = false;
        isAvailable = true;
        objectType = ObjectType.ITEMBOX;
    }
    public void Init(Item _item)
    {
        Init();
        item = _item;
    }
    public override void Active()
    {
        base.Active();
        isAvailable = false;
        ItemManager.Instance.CreateItem(item, this.transform.position);
        Destroy(this.gameObject, 3);
    }

    public void DestroySelf()
    {
        Destroy(item);
        Destroy(gameObject);
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

    public void Init(Item _item)
    {
        Init();
        innerObject = _item;
        sprite = innerObject.GetComponent<SpriteRenderer>().sprite;
    }

    public void ReAlign()
    {
        innerObject.transform.position = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (innerObject as Weapon != null || !isAvailable)
                return;
            innerObject.gameObject.SetActive(true);
            innerObject.GetComponent<Item>().Active();
            Destroy(gameObject);
        }
    }

    public override void Active()
    {
        base.Active();
        Debug.Log("ItemContainer");
        if (innerObject as Weapon != null)
        {
            bool check = PlayerManager.Instance.GetPlayer().GetWeaponManager().PickAndDropWeapon(innerObject);
            if(check)
                Destroy(gameObject);
        }
    }

    public void DestroySelf()
    {
        Destroy(innerObject);
        Destroy(gameObject);
    }
}