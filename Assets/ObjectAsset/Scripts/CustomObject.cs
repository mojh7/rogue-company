using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectType
{
    NONE, UNBREAKABLE, BREAKABLE, PUSHBOX, ITEMBOX,
    VENDINMACHINE, SPAWNER, PORTAL, SNACKBOX, MEDKITBOX,
    SUBSTATION, STOREITEM
}

public class CustomObject : MonoBehaviour
{

    public Vector3 position;
    public ObjectType objectType;
    public Sprite[] sprites;

    protected Sprite sprite;
    protected bool isActive;
    protected bool isAvailable;
    protected bool isAnimate;
    #region components
    protected SpriteRenderer spriteRenderer;
    protected Animator animator;
    protected BoxCollider2D boxCollider;
    protected new Rigidbody2D rigidbody2D;
    protected TextMesh textMesh;
    protected PolygonCollider2D polygonCollider2D;
    #endregion

    public bool GetActive()
    {
        return isActive;
    }

    protected void DestroyAndDeactive()
    {
        Destroy(this);
        this.gameObject.SetActive(false);
    }

    protected void StartAni()
    {
        animator.enabled = true;
    }

    protected void StopAni()
    {
#if UNITY_EDITOR
        animator = GetComponent<Animator>();
#endif
        animator.enabled = false;
        spriteRenderer.sprite = sprite;
    }

    public virtual void Init()
    {
        gameObject.layer = 1;
        SetNullPolygon();
#if UNITY_EDITOR
        spriteRenderer = GetComponent<SpriteRenderer>();
#endif
        spriteRenderer.color = Color.white;
        spriteRenderer.sortingOrder = -Mathf.RoundToInt(transform.position.y * 100);
        StopAni();
        tag = "Wall";
    }

    private void SetNullPolygon()
    {
#if UNITY_EDITOR
        polygonCollider2D = GetComponent<PolygonCollider2D>();
#endif
        polygonCollider2D.pathCount = 1;
        polygonCollider2D.SetPath(0, new Vector2[4] { new Vector2(-.1f, -.1f), new Vector2(.1f, -.1f), new Vector2(.1f, .1f), new Vector2(-.1f, .1f) });
    }
#if UNITY_EDITOR
    public void SetPosition()
    {
        position = new Vector3(transform.position.x, transform.position.y, 0);
    }
#endif
    protected void SetSpriteAndCollider()
    {
        if (sprite)
        {
            spriteRenderer.sprite = sprite;
            List<Vector2> list = new List<Vector2>();
            int num = sprite.GetPhysicsShapeCount();
            polygonCollider2D.pathCount = num;
            for (int i = 0; i < num; i++)
            {
                sprite.GetPhysicsShape(i, list);
                polygonCollider2D.SetPath(i, list.ToArray());
            }
            polygonCollider2D.isTrigger = false;
            polygonCollider2D.enabled = true;
        }
    }

    public void SetAnimate()
    {
        isAnimate = false;
        StopAni();
    }

    public virtual bool GetAvailable()
    {
        return isAvailable;
    }

    public virtual void SetAvailable()
    {
        isAvailable = !isAvailable;
    }

    public virtual bool Active()
    {
        if (!isAvailable)
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
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.bodyType = RigidbodyType2D.Static;
        textMesh = GetComponentInChildren<TextMesh>();
        polygonCollider2D = GetComponent<PolygonCollider2D>();
    }
    #endregion
}

public class NoneRandomSpriteObject : CustomObject
{
    public override void Init()
    {
        base.Init();
        //gameObject.hideFlags = HideFlags.HideInHierarchy;
        isAnimate = false;
        if (sprites != null)
            sprite = sprites[0];
        else
            sprite = null;
        SetSpriteAndCollider();
    }

}

public class RandomSpriteObject : CustomObject
{
    public override void Init()
    {
        base.Init();
        //gameObject.hideFlags = HideFlags.HideInHierarchy;
        isAnimate = false;
        if (sprites != null)
            sprite = sprites[Random.Range(0, sprites.Length)];
        else
            sprite = null;
        SetSpriteAndCollider();
    }

}

public class UnbreakableBox : RandomSpriteObject
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
}

public class BreakalbeBox : RandomSpriteObject
{
    int duration;

    public override void SetAvailable()
    {
    }

    public override void Init()
    {
        base.Init();
        isActive = false;
        isAvailable = false;
        objectType = ObjectType.BREAKABLE;
        duration = 10;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (UtilityClass.CheckLayer(collision.gameObject.layer, 15, 17) && duration > 0)
        {
            duration--;
            if (duration == 0)
            {
                Destruct();
            }
        }
    }

    void Destruct()
    {
        ParticleManager.Instance.PlayParticle("BrokenParticle", this.transform.position, sprite);
        polygonCollider2D.enabled = false;
        gameObject.SetActive(false);
        AStar.TileGrid.Instance.Bake(spriteRenderer);
    }

}

public class VendingMachine : RandomSpriteObject
{
    public override void Init()
    {
        base.Init();
        isActive = false;
        isAvailable = true;
        isAnimate = true;
        objectType = ObjectType.VENDINMACHINE;
    }

    public override bool Active()
    {
        if (base.Active())
        {
            //TODO : 지금은 코인인데 음료수가 들어가야함.
            GameObject coin = new GameObject();
            coin.AddComponent<SpriteRenderer>().sprite = ItemManager.Instance.coinSprite;
            coin.AddComponent<Coin>();
            Vector2 pos = new Vector2(transform.position.x, transform.position.y - .8f);
            ItemManager.Instance.CreateItem(coin.GetComponent<Coin>(), pos);
            return true;
        }
        return base.Active();
    }
}

public class PushBox : RandomSpriteObject
{
    Vector2 oldPosition;
    Vector2 dir;
    Vector3 offset;
    public override void Init()
    {
        base.Init();
        isActive = false;
        isAvailable = true;
        objectType = ObjectType.PUSHBOX;
        offset = new Vector3(0, sprites[0].bounds.size.y * 0.5f, 0);
    }
    public override void SetAvailable()
    {
        isAvailable = true;
    }
    public override bool Active()
    {
        oldPosition = transform.position;
        isActive = true;
        dir = offset + transform.position - PlayerManager.Instance.GetPlayerPosition();
        dir.Normalize();
        rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        StartCoroutine(CoroutinePushed(dir));

        return true;
    }
    //TODO : 캐릭터가 밀리는 문제가 발생
    IEnumerator CoroutinePushed(Vector2 direction)
    {
        float speed = 20;
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
        AStar.TileGrid.Instance.Bake(spriteRenderer.sprite,oldPosition);
        AStar.TileGrid.Instance.Bake(spriteRenderer);
        rigidbody2D.bodyType = RigidbodyType2D.Static;
        spriteRenderer.sortingOrder = -Mathf.RoundToInt(transform.position.y * 100);
        isActive = false;
    }
}

public class Spawner : RandomSpriteObject
{
    int spawnCount = 2;
    int gage;
    public override void Init()
    {
        base.Init();
        isActive = false;
        isAvailable = false;
        polygonCollider2D.enabled = false;
        objectType = ObjectType.SPAWNER;

        gameObject.layer = 0;
    }
    public override bool Active()
    {
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

public class Door : RandomSpriteObject
{
    bool isHorizon;
    Sprite openSprite;
    Sprite closeSprite;

    public override void Init()
    {
        base.Init();
        objectType = ObjectType.NONE;
    }
    public void Init(Sprite _openSprite, Sprite _closeSprite)
    {
        Init();
        openSprite = _openSprite;
        closeSprite = _closeSprite;
        sprite = openSprite;
        SetCollision();
    }
    void SetCollision()
    {
        spriteRenderer.sprite = sprite;
        List<Vector2> list = new List<Vector2>();
        int num = sprite.GetPhysicsShapeCount();
        polygonCollider2D.pathCount = num;
        for (int i = 0; i < num; i++)
        {
            sprite.GetPhysicsShape(i, list);
            polygonCollider2D.SetPath(i, list.ToArray());
        }
        polygonCollider2D.isTrigger = false;
        polygonCollider2D.enabled = true;
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
            StartAni();
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

public class Alert : RandomSpriteObject
{
    public delegate void Del(Vector3 _position, object obj, float amount);
    Del callback;
    object temporary;
    float amount;
    int type = 0;
    public override void Init()
    {
        base.Init();
        isAvailable = false;
        polygonCollider2D.SetPath(0, null);
        objectType = ObjectType.NONE;
    }
    public void Init(Del _call, object temporary, float amount, int type)
    {
        Init();
        callback += _call;
        this.temporary = temporary;
        this.amount = amount;
        this.type = type;
    }
    public override bool Active()
    {
        base.Active();
        StartAni();
        isAnimate = true;
        if (type == 0)
        {
            animator.SetTrigger("skull_alert");
        }
        else
        {
            animator.SetTrigger("circle_alert");
        }
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

        callback(transform.position, temporary, amount);
        DestroyAndDeactive();
    }
}

public class Portal : RandomSpriteObject
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

public class ItemBox : RandomSpriteObject
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
        UtilityClass.Invoke(this, DestroyAndDeactive, 3);
        return true;
    }

    public void DestroySelf()
    {
        if (typeof(Weapon) != item.GetType())
        {
            Destroy(item.gameObject);
        }
        DestroyAndDeactive();
    }
}

public class ItemContainer : RandomSpriteObject
{
    Item innerObject;

    public override void Init()
    {
        base.Init();
        polygonCollider2D.enabled = false;
        polygonCollider2D.isTrigger = true;
        isActive = false;
        isAvailable = true;
        isAnimate = true;
        objectType = ObjectType.NONE;
        tag = "Untagged";
        textMesh.text = "";
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
            if (innerObject.GetType() != typeof(Coin) || !isAvailable)
                return;
            DettachDestroy();
            innerObject.GetComponent<Item>().Active();
        }
    }

    public override bool Active()
    {
        if (innerObject.GetType() == typeof(Weapon))
        {
            bool check = PlayerManager.Instance.GetPlayer().GetWeaponManager().PickAndDropWeapon(innerObject);
            if (check)
            {
                DestroyAndDeactive();
                return true;
            }
        }
        else if(innerObject.GetType() == typeof(UsableItem))
        {
            innerObject.Active();
            DestroyAndDeactive();
            return true;
        }

        return false;
    }

    public void SubAcitve()
    {
        if (innerObject != null)
        {
            innerObject.SubActive();
        }
    }

    public override void IndicateInfo()
    {
        textMesh.text = innerObject.GetName();
    }

    public override void DeIndicateInfo()
    {
        textMesh.text = "";
    }

    public void DettachDestroy()
    {
        if (innerObject != null)
            innerObject.transform.parent = null;
        DestroyAndDeactive();
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

public class FallRockTrap : RandomSpriteObject
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
        gameObject.layer = 0;
        polygonCollider2D.pathCount = num;
        for (int i = 0; i < num; i++)
        {
            tempSprite.GetPhysicsShape(i, list);
            GetComponent<PolygonCollider2D>().SetPath(i, list.ToArray());
        }
        polygonCollider2D.isTrigger = false;
    }
    public override bool Active()
    {
        this.gameObject.AddComponent<Alert>();
        this.gameObject.GetComponent<Alert>().sprites = null;
        this.gameObject.GetComponent<Alert>().Init(CallBack, null, 0, 0);
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
    void CallBack(Vector3 _position, object temporary, float amount)
    {
        GameObject obj = ResourceManager.Instance.objectPool.GetPooledObject();
        obj.AddComponent<Rock>();
        obj.GetComponent<Rock>().sprites = new Sprite[1] { tempSprite };
        obj.GetComponent<Rock>().Init();
        obj.transform.position = _position;
        obj.GetComponent<Rock>().Active();
    }
}

public class Rock : RandomSpriteObject
{
    int duration;

    public override void Init()
    {
        base.Init();
        isAvailable = true;
        polygonCollider2D.enabled = true;
        duration = 10;
    }
    public override bool Active()
    {
        StartCoroutine(Dropping());
        return base.Active();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = collision.gameObject;

        if (UtilityClass.CheckLayer(obj.gameObject.layer, 16, 13) && isAvailable)
        {
            isAvailable = false;
            Vector2 dir = obj.transform.position - transform.position;
            obj.GetComponent<Character>().Attacked(dir, transform.position, 1, 200, 0);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (UtilityClass.CheckLayer(collision.gameObject.layer, 15, 17) && duration > 0)
        {
            duration--;
            if (duration == 0)
            {
                Destruct();
            }
        }
    }
    void Destruct()
    {
        ParticleManager.Instance.PlayParticle("BrokenParticle", this.transform.position, sprite);
        polygonCollider2D.enabled = false;
        gameObject.SetActive(false);
        AStar.TileGrid.Instance.Bake(spriteRenderer);
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

public class SnackBox : NoneRandomSpriteObject
{

    public override void Init()
    {
        base.Init();
        polygonCollider2D.enabled = true;
        isActive = false;
        isAvailable = true;
        isAnimate = false;
        objectType = ObjectType.SNACKBOX;
    }

    public override void SetAvailable()
    {
        return;
    }

    public override bool Active()
    {
        if (base.Active())
        {
            //stamina recovery
            isAvailable = false;
            sprite = sprites[1];
            spriteRenderer.sprite = sprite;
            return true;
        }
        return false;
    }

    public override void IndicateInfo()
    {
        textMesh.text = "간식을 드시겠습니까?";
    }

    public override void DeIndicateInfo()
    {
        textMesh.text = "";
    }
}

public class MedkitBox : NoneRandomSpriteObject
{
    public override void Init()
    {
        base.Init();
        polygonCollider2D.enabled = true;
        isActive = false;
        isAvailable = true;
        isAnimate = false;
        objectType = ObjectType.MEDKITBOX;
    }

    public override void SetAvailable()
    {
        return;
    }

    public override bool Active()
    {
        if (base.Active())
        {
            //Item Drop
            isAvailable = false;
            sprite = sprites[1];
            spriteRenderer.sprite = sprite;
            return true;
        }
        return false;
    }

    public override void IndicateInfo()
    {
        textMesh.text = "약이 들어있습니다.";
    }

    public override void DeIndicateInfo()
    {
        textMesh.text = "";
    }
}

public class SubStation : NoneRandomSpriteObject
{
    public override bool Active()
    {
        return base.Active();
    }
}

public class StoreItem : CustomObject
{
    Item innerObject;

    public override void Init()
    {
        base.Init();
        polygonCollider2D.isTrigger = true;
        isActive = false;
        isAvailable = true;
        isAnimate = true;
        objectType = ObjectType.STOREITEM;
    }

    public override void SetAvailable()
    {
        if(isAvailable)
        {
            innerObject = ObjectPoolManager.Instance.CreateUsableItem(); 
            sprite = innerObject.GetComponent<SpriteRenderer>().sprite;
            ReAlign();
        }
    }

    void ReAlign()
    {
        innerObject.transform.parent = transform;
        innerObject.transform.localPosition = Vector3.zero;
    }

    public override bool Active()
    {
        if(base.Active())
        {
            if(GameDataManager.Instance.GetCoin() >= innerObject.GetValue())
            {
                isAvailable = false;
                GameDataManager.Instance.ReduceCoin(innerObject.GetValue());
                ItemManager.Instance.CreateItem(innerObject, transform.position, new Vector2(Random.Range(-1, 2), 3));
            }
            return true;
        }
        return false;
    }

    public override void IndicateInfo()
    {
        if(GameDataManager.Instance.GetCoin() >= innerObject.GetValue())
        {
            textMesh.text = innerObject.GetName();
        }
        else
        {
            textMesh.text = "돈이 부족합니다.";
        }
    }

    public override void DeIndicateInfo()
    {
        textMesh.text = "";
    }
}