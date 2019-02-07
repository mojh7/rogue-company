using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 일단 가지고 있던 메모리풀 쓰고 있는 중... - 장현

public enum UsableItemType { CLOTHING, ETC, FOOD, MEDICAL, MISC, PET }

public class ObjectPoolManager : MonoBehaviourSingleton<ObjectPoolManager> {

    // 무기 오브젝트풀은 일단 보류

    #region variables

    public GameObject weaponPrefab;
    [SerializeField]
    private int initWeaponNumMax;
    [SerializeField]
    private MemoryPool weaponPool;

    public GameObject bulletPrefab;
    [SerializeField]
    private int initBulletNumMax;
    [SerializeField]
    private MemoryPool bulletPool;

    public GameObject itemPrefab;
    [SerializeField]
    private int initItemNumMax;
    [SerializeField]
    private MemoryPool itemPool;

    public GameObject trailRendererPrefab;
    [SerializeField]
    private int initTrailRendererNumMax;
    [SerializeField]
    private MemoryPool trailRendererPool;


    [SerializeField] private Transform weaponsTrasnform;
    [SerializeField] private Transform bulletsTrasnform;
    [SerializeField] private Transform itemsTrasnform;
    [SerializeField] private Transform trailRendererTrasnform;
    #endregion

    #region getter
    public MemoryPool WeaponPool { get { return weaponPool; } }
    public MemoryPool BulletPool { get { return bulletPool; } }
    public Transform GetWeaponsTrasnform() { return weaponsTrasnform; }
    public Transform GetBulletsTrasnform() { return bulletsTrasnform; }

    public Transform GetTrailRendererTransform() { return trailRendererTrasnform; }
    #endregion


    private void Awake()
    {
        // 오브젝트 풀 초기화
        weaponPool = new MemoryPool(weaponPrefab, initWeaponNumMax, weaponsTrasnform, "Weapon_");
        bulletPool = new MemoryPool(bulletPrefab, initBulletNumMax, bulletsTrasnform, "Bullet_");
        itemPool = new MemoryPool(itemPrefab, initItemNumMax, itemsTrasnform, "item_");
        trailRendererPool = new MemoryPool(trailRendererPrefab, initTrailRendererNumMax, trailRendererTrasnform, "trail_");
    }

    #region function
    // 나중에 오브젝트 풀 바껴도 object Create, Delete 함수 내부만 바꾸면 되서 함수 새로 만듬.

    #region createObject

    public Weapon CreateWeapon()
    {
        GameObject createdObj = weaponPool.NewItem();
        createdObj.GetComponent<Weapon>().Init(Random.Range(0, WeaponsData.Instance.GetWeaponInfosLength()));
        return createdObj.GetComponent<Weapon>();
    }

    public Weapon CreateWeapon(WeaponInfo weaponInfo)
    {
        GameObject createdObj = weaponPool.NewItem();
        createdObj.GetComponent<Weapon>().Init(weaponInfo);
        return createdObj.GetComponent<Weapon>();
    }

    public Weapon CreateWeapon(Rating rating)
    {
        if (Rating.NORATING == rating)
            return null;
        GameObject createdObj = weaponPool.NewItem();
        createdObj.GetComponent<Weapon>().Init(WeaponsData.Instance.GetWeaponInfo(rating));
        return createdObj.GetComponent<Weapon>();
    }

    /// <summary>
    /// weaponId 정보로 weapon item 생성
    /// </summary>
    /// <param name="weaponId"></param>
    /// <returns></returns>
    public Item CreateWeapon(int weaponId)
    {
        GameObject createdObj = weaponPool.NewItem();
        createdObj.GetComponent<Weapon>().Init(weaponId);
        return createdObj.GetComponent<Item>();
    }

    /// <summary> bullet 생성 후 반환 </summary>
    public GameObject CreateBullet()
    {
        return bulletPool.NewItem();
    }

    /// <summary>
    /// UsableItem 생성
    /// </summary>
    /// <param name="type">생성할 아이템 타입</param>
    /// <param name="usableItemId">아이템 id, 미 기입시 랜덤 생성</param>
    /// <returns>생성된 오브젝트의 UsableItem Class 반환</returns>
    public UsableItem CreateUsableItem(UsableItemType type, int usableItemId = -1)
    {
        GameObject createdObj = itemPool.NewItem();
        UsableItem usableItem = createdObj.GetComponent<UsableItem>();
        switch (type)
        {
            case UsableItemType.CLOTHING:
                usableItem.Init(ItemsData.Instance.GetClothingItemInfo(usableItemId));
                break;
            case UsableItemType.ETC:
                usableItem.Init(ItemsData.Instance.GetEtcItemInfo(usableItemId));
                break;
            case UsableItemType.FOOD:
                usableItem.Init(ItemsData.Instance.GetFoodItemInfo(usableItemId));
                break;
            case UsableItemType.MEDICAL:
                usableItem.Init(ItemsData.Instance.GetMedicalItemInfo(usableItemId));
                break;
            case UsableItemType.MISC:
                usableItem.Init(ItemsData.Instance.GetMiscItemInfo(usableItemId));
                break;
            case UsableItemType.PET:
                usableItem.Init(ItemsData.Instance.GetPetItemInfo(usableItemId));
                break;
            default:
                break;
        }

        return usableItem;
    }

    public UsableItem CreateUsableItem()
    {
        if (itemPool == null)
            return null;
        GameObject createdObj = itemPool.NewItem();
        UsableItem usableItem = createdObj.GetComponent<UsableItem>();
        UsableItemType type = (UsableItemType)Random.Range(0, System.Enum.GetNames(typeof(UsableItemType)).Length - 1);
        //TODO : Pet아이템이 없어서 legth -1을 넣어놓음
        int usableItemId = -1;
        switch (type)
        {
            case UsableItemType.CLOTHING:
                usableItem.Init(ItemsData.Instance.GetClothingItemInfo(usableItemId));
                break;
            case UsableItemType.ETC:
                usableItem.Init(ItemsData.Instance.GetEtcItemInfo(usableItemId));
                break;
            case UsableItemType.FOOD:
                usableItem.Init(ItemsData.Instance.GetFoodItemInfo(usableItemId));
                break;
            case UsableItemType.MEDICAL:
                usableItem.Init(ItemsData.Instance.GetMedicalItemInfo(usableItemId));
                break;
            case UsableItemType.MISC:
                usableItem.Init(ItemsData.Instance.GetMiscItemInfo(usableItemId));
                break;
            case UsableItemType.PET:
                usableItem.Init(ItemsData.Instance.GetPetItemInfo(usableItemId));
                break;
            default:
                break;
        }

        return usableItem;
    }

    public UsableItem CreateUsableItem(Rating rating)
    {
        if (itemPool == null)
            return null;
        GameObject createdObj = itemPool.NewItem();
        UsableItem usableItem = createdObj.GetComponent<UsableItem>();
        UsableItemType type = (UsableItemType)Random.Range(0, System.Enum.GetNames(typeof(UsableItemType)).Length - 1);
        //TODO : Pet아이템이 없어서 legth -1을 넣어놓음
        int usableItemId = -1;
        switch (type)
        {
            case UsableItemType.CLOTHING:
                usableItem.Init(ItemsData.Instance.GetClothingItemInfo(usableItemId));
                break;
            case UsableItemType.ETC:
                usableItem.Init(ItemsData.Instance.GetEtcItemInfo(usableItemId));
                break;
            case UsableItemType.FOOD:
                usableItem.Init(ItemsData.Instance.GetFoodItemInfo(usableItemId));
                break;
            case UsableItemType.MEDICAL:
                usableItem.Init(ItemsData.Instance.GetMedicalItemInfo(usableItemId));
                break;
            case UsableItemType.MISC:
                usableItem.Init(ItemsData.Instance.GetMiscItemInfo(usableItemId));
                break;
            case UsableItemType.PET:
                usableItem.Init(ItemsData.Instance.GetPetItemInfo(usableItemId));
                break;
            default:
                break;
        }

        return usableItem;
    }

    public TrailRenderer CreateTrailRenderer()
    {
        GameObject createdObj = trailRendererPool.NewItem();
        return createdObj.GetComponent<TrailRenderer>();
    }
    #endregion


    #region deleteObject

    /// <summary> weapon object 삭제(회수) </summary>
    public void DeleteWeapon(GameObject obj)
    {
        weaponPool.RemoveItem(obj);
    }

    /// <summary> bullet object 삭제(회수) </summary>
    public void DeleteBullet(GameObject obj)
    {
        bulletPool.RemoveItem(obj);
    }

    /// <summary> UsableItem object 삭제(회수) </summary>
    public void DeleteUsableItem(GameObject obj)
    {
        itemPool.RemoveItem(obj);
    }

    /// <summary> UsableItem object 삭제(회수) </summary>
    public void DeleteTrailRenderer(GameObject obj)
    {
        trailRendererPool.RemoveItem(obj, true, false);
    }

    public void ClearWeapon()
    {
        weaponPool.ClearItem();
    }
    public void ClearBullet()
    {
        bulletPool.ClearItem();
    }
    public void ClearUsableItem()
    {
        itemPool.ClearItem();
    }

    #endregion


    /*private void OnApplicationQuit()
    {
        //weaponPool.Dispose();
        //bulletPool.Dispose();
    }*/
    #endregion 
}