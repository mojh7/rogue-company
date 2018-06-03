using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 일단 가지고 있던 메모리풀 쓰고 있는 중... - 장현

public class ObjectPoolManager : MonoBehaviourSingleton<ObjectPoolManager> {

    // 무기 오브젝트풀은 일단 보류

    #region variables

    public Transform tempObj;

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

    // effect 아직 안 옮김
    public GameObject effectPrefab;
    [SerializeField]
    private int initEffectNumMax;
    [SerializeField]
    private MemoryPool effectPool;

    #endregion

    #region getter
    public MemoryPool WeaponPool { get { return weaponPool; } }
    public MemoryPool BulletPool { get { return bulletPool; } }
    public MemoryPool EffectPool { get { return effectPool; } }
    #endregion


    private void Awake()
    {
        // 오브젝트 풀 초기화

        // weapon 오브젝트풀 초기화
        weaponPool = new MemoryPool(weaponPrefab, initWeaponNumMax);
        // bullet 오브젝트풀 초기화
        bulletPool = new MemoryPool(bulletPrefab, initBulletNumMax);
        // effect 오브젝트풀 초기화
        //effectPool = new MemoryPool(effectPrefab, initEffectNumMax);
    }

    #region function
    // 나중에 오브젝트 풀 바껴도 object Create, Delete 함수 내부만 바꾸면 되서 함수 새로 만듬.

    #region createObject
    public Weapon CreateWeapon()
    {
        GameObject createdObj = weaponPool.NewItem();
        return createdObj.GetComponent<Weapon>();
    }

    public Item CreateWeapon(int weaponId)
    {
        GameObject createdObj = weaponPool.NewItem();
        createdObj.GetComponent<Weapon>().Init(weaponId);
        return createdObj.GetComponent<Item>();
    }
    /// <summary> 해당 weapon Id의 정보를 가진 weapon 생성 </summary>
    public GameObject CreateWeapon(int weaponId, Vector3 pos)
    {
        GameObject createdObj = weaponPool.NewItem();
        createdObj.GetComponent<Transform>().position = pos;
        createdObj.GetComponent<Weapon>().Init(weaponId);
        return createdObj;
    }

    /// <summary> 해당 weapon Id의 정보를 가진 weapon 생성 및 parent 설정 </summary>
    public GameObject CreateWeapon(int weaponId, Vector3 pos, Transform parent, bool worldPositionStays = true)
    {
        GameObject createdObj = weaponPool.NewItem();
        createdObj.GetComponent<Weapon>().Init(weaponId, OwnerType.Player);
        createdObj.GetComponent<Transform>().position = pos;
        createdObj.GetComponent<Transform>().SetParent(parent, worldPositionStays);
        return createdObj;
    }

    /// <summary> bullet 생성 후 반환 </summary>
    public GameObject CreateBullet()
    {
        return bulletPool.NewItem();
    }

    /// <summary> effect 생성 </summary>
    //public void CreateEffect(int id, Vector3 pos)
    //{
    //    if (id < 0) return;
    //    pos.z = 0;
    //    GameObject createdObj = effectPool.NewItem();
    //    createdObj.GetComponent<Effect>().Init(id, pos);
    //}
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

    /// <summary> effect object 삭제(회수) </summary>
    //public void DeleteEffect(GameObject obj)
    //{
    //    effectPool.RemoveItem(obj);
    //}



    public void ClearWeapon()
    {
        weaponPool.ClearItem();
    }
    public void ClearBullet()
    {
        bulletPool.ClearItem();
    }
    //public void ClearEffect()
    //{
    //    effectPool.ClearItem();
    //}

    #endregion


    private void OnApplicationQuit()
    {
        //weaponPool.Dispose();
        //bulletPool.Dispose();
        //effectPool.Dispose();

    }
    #endregion 
}