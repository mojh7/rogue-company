using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviourSingleton<ItemManager> {
    #region boxPercentage
    private int[,] iEventboxPercentage =
    {   //not,S,A,B,C,D,E
/*floor*/{  0,    5,    10,     20,     80,     20,    10 },
         {  0,    8,    10,     10,     80,     10,    10 },
         {  0,   10,    10,     80,     10,     10,    10 },
         {  0,   10,    80,     10,     10,     10,    10 },
         {  0,   20,    90,     10,     10,     10,    10 }
    };

    private int[,] iBossboxPercentage =
   {   //not,S,A,B,C,D,E
/*floor*/{  0,    5,    10,     10,     70,     30,    10 },
         {  0,    5,    10,     70,     30,     10,    10 },
         {  0,    5,    70,     30,     10,     10,    10 },
         {  0,   50,    50,     10,     10,     10,    10 },
         {  0,  100,    30,     20,     10,     10,    10 }
    };

    private int[,] iRestboxPercentage =
    {   //not,S,A,B,C,D,E
/*floor*/{  0,    2,     3,      4,      5,     40,    60 },
         {  0,    4,     7,      8,     10,     80,    10 },
         {  0,    6,     9,     10,     80,     10,     5 },
         {  0,   10,    10,     80,     10,      5,     5 },
         {  0,   10,    40,     60,      5,      5,     5 }
    };
    #endregion
    [SerializeField]
    private Sprite[] boxSprites;
    [SerializeField]
    private Sprite coinSprite;
    [SerializeField]
    private Sprite cardSprite;
    [SerializeField]
    private Sprite ammoSprite;


    Queue<GameObject> objs;
    Queue<ItemContainer> withdraws;
    #region UnityFunc
    private void Awake()
    {
        objs = new Queue<GameObject>();
        withdraws = new Queue<ItemContainer>();
    }
    #endregion
    #region Func
    public void DropAmmo(Vector3 pos)
    {
        GameObject ammo = ResourceManager.Instance.itemPool.GetPooledObject();
        ammo.GetComponent<SpriteRenderer>().sprite = ItemManager.Instance.cardSprite;
        ammo.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        ammo.AddComponent<Ammo>();
        ItemManager.Instance.CreateItem(ammo.GetComponent<Ammo>(), pos, new Vector2(Random.Range(-1f, 1f), Random.Range(3, 8)));
    }
    public void DropCard(Vector3 pos)
    {
        GameObject card = ResourceManager.Instance.itemPool.GetPooledObject();
        card.GetComponent<SpriteRenderer>().sprite = ItemManager.Instance.cardSprite;
        card.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        card.AddComponent<Card>();
        ItemManager.Instance.CreateItem(card.GetComponent<Card>(), pos, new Vector2(Random.Range(-1f, 1f), Random.Range(3, 8)));
    }
    public Coin DropCoin()
    {
        GameObject coin = ResourceManager.Instance.itemPool.GetPooledObject();
        coin.GetComponent<SpriteRenderer>().sprite = ItemManager.Instance.coinSprite;
        coin.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        coin.AddComponent<Coin>();

        return coin.GetComponent<Coin>();
    }
    public void DeleteObjs()
    {
        if (objs == null)
            return;
        while (objs.Count > 0)
        {
            GameObject gameObject = objs.Dequeue();
            if (gameObject != null)
            {
                if(gameObject.GetComponent<ItemContainer>() != null)
                    gameObject.GetComponent<ItemContainer>().DestroySelf();
                if (gameObject.GetComponent<ItemBox>() != null)
                    gameObject.GetComponent<ItemBox>().DestroySelf();
            }
        }
    }
    public GameObject CreateItem(Item _item, Vector3 _position, params Vector2[] dest)
    {
        GameObject obj = ResourceManager.Instance.objectPool.GetPooledObject();
        if (obj != null)
        {
            obj.transform.position = _position;
            obj.AddComponent<ItemContainer>().Init(_item);
            objs.Enqueue(obj);
            if (_item.GetType() == typeof(Coin) || _item.GetType() == typeof(Card))
            {
                obj.GetComponent<ItemContainer>().IsCoin();
                withdraws.Enqueue(obj.GetComponent<ItemContainer>());
            }
            if (dest.Length == 0)
                StartCoroutine(CoroutineDropping(obj, new Vector2(Random.Range(-0.5f, 0.5f), 5)));
            else
                StartCoroutine(CoroutineDropping(obj, dest[0]));

            return obj;
        }
        return null;
    }
    public void CollectItem()
    {
        if (withdraws == null)
            return;
        while (withdraws.Count > 0)
        {
            ItemContainer itemContainer = withdraws.Dequeue();
            if (itemContainer != null)
            {
                itemContainer.DettachDestroy();
                itemContainer.SubAcitve();
            }
        }
    }

    public void CallItemBox(Vector2 _position, RoomType roomType)
    {
        GameObject obj = ResourceManager.Instance.objectPool.GetPooledObject();
        int floor = InGameManager.Instance.GetFloor();
        Rating rating = Rating.NORATING;
        switch (roomType)
        {
            case RoomType.BOSS:
                rating = GetRating(floor, iBossboxPercentage);
                break;
            case RoomType.REST:
                rating = GetRating(floor, iRestboxPercentage);
                break;
            default:
                return;
        }
        Item item = ObjectPoolManager.Instance.CreateWeapon(rating);
        ParticleManager.Instance.PlayParticle("Event", _position);
        objs.Enqueue(obj);
        obj.transform.position = _position;
        obj.AddComponent<ItemBox>();
        obj.GetComponent<ItemBox>().sprites = new Sprite[1] { GetItemRatingSprite(item.GetRating()) };
        obj.GetComponent<ItemBox>().Init(item);
        obj.transform.position = new Vector2(obj.transform.position.x, obj.transform.position.y);
    }

    public void SetItemBox(ItemBox itemBox)
    {
        int floor = InGameManager.Instance.GetFloor();
        Rating rating = Rating.NORATING;
        rating = GetRating(floor, iEventboxPercentage);

        Item item = ObjectPoolManager.Instance.CreateWeapon(rating);
        itemBox.sprites = new Sprite[1] { GetItemRatingSprite(item.GetRating()) };
        itemBox.Init(item);
    }
    public Rating GetStoreItemRating()
    {
        int floor = InGameManager.Instance.GetFloor();

        return GetRating(floor, iRestboxPercentage);
    }
    private Rating GetRating(int floor,int [,] array)
    {
        int total = 0;
        int length = System.Enum.GetValues(typeof(Rating)).Length;
        for (int i = 0; i < length; i++)
        {
            total += array[floor, i] + PlayerBuffManager.Instance.BuffManager.InGameTargetEffectTotal.rateUpperPercent.percent[i];
        }
        float randomPoint = Random.value * total;
        for (int i = 0; i < length; i++)
        {
            if (randomPoint < array[floor,i] + PlayerBuffManager.Instance.BuffManager.InGameTargetEffectTotal.rateUpperPercent.percent[i])
            {
                return (Rating)i;
            }
            else
            {
                randomPoint -= array[floor, i] + PlayerBuffManager.Instance.BuffManager.InGameTargetEffectTotal.rateUpperPercent.percent[i];
            }
        }

        return Rating.NORATING;
    }
    private Sprite GetItemRatingSprite(Rating rating)
    {
        switch (rating)
        {
            case Rating.S:
                return boxSprites[3];
            case Rating.A:
                return boxSprites[2];
            case Rating.B:
                return boxSprites[1];
            default:
            case Rating.NORATING:
            case Rating.C:
            case Rating.D:
            case Rating.E:
                return boxSprites[0];
        }
    }
    #endregion
    #region Coroutine
    IEnumerator CoroutineDropping(GameObject _object, Vector2 _vector)
    {
        int g = 20;
        float lowerLimit = _object.transform.position.y;
        float elapsed_time = 0;
        float sX = _object.transform.position.x;
        float sY = _object.transform.position.y;
        float vX = _vector.x;
        float vY = _vector.y;
        while (true)
        {
            if (_object == null)
                break;
            elapsed_time += Time.deltaTime;
            float x = sX + vX * elapsed_time;
            float y = sY + vY * elapsed_time - (0.5f * g * elapsed_time * elapsed_time);
            _object.transform.position = new Vector2(x, y);
            if (_object.transform.position.y <= lowerLimit)
                break;
            yield return YieldInstructionCache.WaitForEndOfFrame;
        }
        if (_object != null)
        {
            _object.GetComponent<PolygonCollider2D>().enabled = true;
        }
    }
    #endregion
}
