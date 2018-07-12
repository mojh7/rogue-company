using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectType { NONE, UNBREAKABLE, BREAKABLE, PUSHBOX, ITEMBOX, VENDINMACHINE, SPAWNER, PORTAL}

public class CustomObject : MonoBehaviour {

    public Vector3 position;
    public ObjectType objectType;
    public Sprite sprite;
    public Sprite[] sprites;
    protected bool isActive;
    protected bool isAvailable;
    protected bool isAnimate;
    protected SpriteRenderer spriteRenderer;
    protected Animator animator;
    protected BoxCollider2D boxCollider;
    protected new Rigidbody2D rigidbody2D;
    protected TextMesh textMesh;

    public bool GetAvailable() { return isAvailable; }
    public bool GetActive() { return isActive; }

    public virtual void Init()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.bodyType = RigidbodyType2D.Static;
        textMesh = GetComponentInChildren<TextMesh>();
#if UNITY_EDITOR
        spriteRenderer = GetComponent<SpriteRenderer>();
#endif
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
        spriteRenderer.sortingOrder = -Mathf.RoundToInt(transform.position.y * 100);
    }

    public void SetPosition()
    {
        position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    public virtual void SetAvailable()
    {
        isAvailable = !isAvailable;
    }

    public void SetAnimate()
    {
        isAnimate = false;
    }

    public virtual bool Active()
    {
        if (!isAvailable || !isAnimate)
            return false;
        isActive = true;
        return true;
    }

    public virtual void IndicateInfo() { }

    public virtual void DeIndicateInfo() { }
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
}

public class PushBox : CustomObject
{
    Vector2 dir;
    public override void Init()
    {
        base.Init();
        isActive = false;
        isAvailable = true;
        objectType = ObjectType.PUSHBOX;
    }
    public override void SetAvailable()
    {
        isAvailable = true;
    }
    public override bool Active()
    {
        isActive = true;
        dir = transform.position - PlayerManager.Instance.GetPlayerPosition();
        dir.Normalize();
        rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        StartCoroutine(CoroutinePushed(dir));

        return true;
    }
    //TODO : 캐릭터가 밀리는 문제가 발생
    IEnumerator CoroutinePushed(Vector2 direction)
    {
        float speed = 10;
        rigidbody2D.velocity = speed * direction;
        float time = 0.1f;
        Vector2 start = rigidbody2D.velocity;
        while (rigidbody2D.velocity.sqrMagnitude > 1)
        {
            rigidbody2D.velocity = Vector2.Lerp(rigidbody2D.velocity, Vector2.zero, Time.deltaTime / time);
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
        }
        StopMove();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && isActive)
            Attack(collision);
    }

    void Attack(Collision2D collision)
    {
        collision.gameObject.GetComponent<Enemy>().Attacked(dir, transform.position, 1, 100, 0);
        StopMove();
    }

    void StopMove()
    {
        rigidbody2D.bodyType = RigidbodyType2D.Static;
        spriteRenderer.sortingOrder = -Mathf.RoundToInt(transform.position.y * 100);
        isActive = false;
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
        gameObject.tag = "Untagged";
        gameObject.layer = 0;
    }
    public override bool Active()
    {
        base.Active();
        StartCoroutine(SpawnProcess());
        return true;
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
    public override bool Active()
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

        return true;
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
    public override bool Active()
    {
        base.Active();
        isAnimate = true;
        animator.SetTrigger("alert_indicator");
        StartCoroutine(CheckAnimate());

        return true;
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
    public override bool Active()
    {
        base.Active();
        isAvailable = false;
        InGameManager.Instance.GoUpFloor();
        Debug.Log("PlayerEnd");

        return true;
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
        item.gameObject.SetActive(false);
    }
    public override bool Active()
    {
        base.Active();
        isAvailable = false;
        item.gameObject.SetActive(true);
        ItemManager.Instance.CreateItem(item, this.transform.position);
        Destroy(this.gameObject, 3);

        return true;
    }

    public void DestroySelf()
    {
        if (typeof(Weapon) != item.GetType())
        {
            Destroy(item.gameObject);
        }
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
        isAnimate = true;
        objectType = ObjectType.NONE;
        gameObject.tag = "Untagged";
        gameObject.layer = 0;
    }

    public void Init(Item _item)
    {
        Init();
        innerObject = _item;
        sprite = innerObject.GetComponent<SpriteRenderer>().sprite;
        ReAlign();
    }

    void ReAlign()
    {
        innerObject.transform.parent = transform;
        innerObject.transform.localPosition = Vector3.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactor"))
        {
            if (innerObject as Weapon != null || !isAvailable)
                return;
            DettachDestroy();
            innerObject.GetComponent<Item>().Active();
        }
    }

    public override bool Active()
    {
        base.Active();
        if (innerObject as Weapon != null)
        {
            bool check = PlayerManager.Instance.GetPlayer().GetWeaponManager().PickAndDropWeapon(innerObject);
            if (check)
            {
                Destroy(gameObject);
                return true;
            }
        }

        return false;
    }

    public override void IndicateInfo()
    {
        base.IndicateInfo();
        textMesh.text = innerObject.GetName();
    }

    public override void DeIndicateInfo()
    {
        base.DeIndicateInfo();
        textMesh.text = "";
    }

    void DettachDestroy()
    {
        if(innerObject != null)
            innerObject.transform.parent = null;
        Destroy(gameObject);
    }

    public void DestroySelf()
    {
        // Debug.Log("inner Type : " + innerObject.GetType());
        // destroy말고 오브젝트 풀에서 회수 처리 일괄적으로 하는 것 더 생길 수 있겠지만
        // 일단 무기만 적용, 무기가 아닌 거(코인)이면 destroy
        if (typeof(Weapon) != innerObject.GetType())
        {
            Destroy(innerObject.gameObject);
        }
        DettachDestroy();
    }
}

public class FallRockTrap : CustomObject
{
    Sprite tempSprite;
    public override void Init()
    {
        base.Init();
    }
    public void Init(Sprite sprite)
    {
        Init();
        isAvailable = true;
        this.tempSprite = sprite;
        List<Vector2> list = new List<Vector2>();
        int num = tempSprite.GetPhysicsShapeCount();
        GetComponent<PolygonCollider2D>().pathCount = num;
        for (int i = 0; i < num; i++)
        {
            tempSprite.GetPhysicsShape(i, list);
            GetComponent<PolygonCollider2D>().SetPath(i, list.ToArray());
        }
        GetComponent<PolygonCollider2D>().isTrigger = false;
    }
    public override bool Active()
    {
        this.gameObject.AddComponent<Alert>();
        this.gameObject.GetComponent<Alert>().sprite = null;
        this.gameObject.GetComponent<Alert>().Init(CallBack);
        this.gameObject.GetComponent<Alert>().Active();
        Destroy(this);
        return true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactor") && isAvailable)
        {
            isAvailable = false;
            Active();
        }
    }
    void CallBack(Vector3 _position)
    {
        GameObject obj = Object.Instantiate(ResourceManager.Instance.ObjectPrefabs);
        obj.AddComponent<Rock>();
        obj.GetComponent<Rock>().Init();
        obj.GetComponent<Rock>().sprite = tempSprite;
        obj.transform.position = _position;
        obj.GetComponent<Rock>().Active();
    }
}

public class Rock : CustomObject
{
    public override void Init()
    {
        base.Init();
        isAvailable = true;
        GetComponent<PolygonCollider2D>().enabled = false;
    }
    public override bool Active()
    {
        StartCoroutine(Dropping());
        return base.Active();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = collision.gameObject;
        if (( obj.CompareTag("Player") || obj.CompareTag("Enemy")) && isAvailable)
        {
            isAvailable = false;
            Vector2 dir = obj.transform.position - transform.position;
            obj.GetComponent<Character>().Attacked(dir, transform.position, 1, 200, 0);
        }
    }

    IEnumerator Dropping()
    {
        float lowerLimit = transform.position.y;
        transform.position = new Vector2(transform.position.x, transform.position.y + 1.5f);
        float elapsed_time = 0;
        float sX = transform.position.x;
        float sY = transform.position.y;
        while (true)
        {
            elapsed_time += Time.deltaTime;
            float x = sX;
            float y = sY - (0.5f * 20 * elapsed_time * elapsed_time);
            transform.position = new Vector2(x, y);
            if (transform.position.y <= lowerLimit)
                break;
            yield return YieldInstructionCache.WaitForEndOfFrame;
        }
        GetComponent<PolygonCollider2D>().enabled = true;
        spriteRenderer.sortingOrder = -Mathf.RoundToInt(transform.position.y * 100);
        yield return YieldInstructionCache.WaitForEndOfFrame;

        isAvailable = false;
    }

}