using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 일단 가지고 있던 메모리풀 쓰고 있는 중... - 장현

public class ObjectPoolManager : MonoBehaviourSingleton<ObjectPoolManager> {

    #region variables

    public GameObject weaponPrefab;
    public GameObject bulletPrefab;
    public GameObject effectPrefab;

    [SerializeField]
    private int weaponNumMax;
    [SerializeField]
    private MemoryPool weaponPool;

    [SerializeField]
    private int bulletNumMax;
    [SerializeField]
    private MemoryPool bulletPool;

    // effect 아직...
    [SerializeField]
    private int effectNumMax;
    [SerializeField]
    private MemoryPool effectPool;

    #endregion

    #region getter
    public MemoryPool WeaponPool { get { return weaponPool; } }
    public MemoryPool BulletPool { get { return bulletPool; } }
    public MemoryPool EffectPool { get { return effectPool; } }
    #endregion


    void Start () {
        // 오브젝트 풀 초기화

        // weapon 오브젝트풀 초기화
        weaponPool = new MemoryPool(weaponPrefab, weaponNumMax);
        // bullet 오브젝트풀 초기화
        bulletPool = new MemoryPool(bulletPrefab, bulletNumMax);
        // effect 오브젝트풀 초기화
        //effectPool = new MemoryPool(bulletPrefab, bulletNumMax);
    }

    #region function
    // 나중에 오브젝트 풀 바껴도 object Create, Delete 함수 내부만 바꾸면 되서 함수 새로 만듬.

    #region createObject
    /// <summary>
    /// 해당 weapon Id의 정보를 가진 weapon 생성
    /// </summary>
    /// <param name="weaponId">생성할 weapon의 고유 Id</param>
    /// <param name="pos">생성할 위치</param>
    /// <returns>생성된 weapon 오브젝트</returns>
    public GameObject CreateWeapon(int weaponId, Vector3 pos)
    {
        GameObject createdObj = weaponPool.NewItem();
        createdObj.GetComponent<Transform>().position = pos;
        createdObj.GetComponent<Weapon>().Init(weaponId);
        return createdObj;
    }

    /// <summary>
    /// 해당 weapon Id의 정보를 가진 weapon 생성 및 parent 설정
    /// </summary>
    /// <param name="weaponId"></param>
    /// <param name="pos"></param>
    /// <param name="parent"></param>
    /// <param name="worldPositionStays"></param>
    /// <returns></returns>
    public GameObject CreateWeapon(int weaponId, Vector3 pos, Transform parent, bool worldPositionStays = true)
    {
        GameObject createdObj = weaponPool.NewItem();
        createdObj.GetComponent<Weapon>().Init(weaponId);
        createdObj.GetComponent<Transform>().position = pos;
        createdObj.GetComponent<Transform>().SetParent(parent, worldPositionStays);
        return createdObj;
    }

    /// <summary>
    /// bullet 생성
    /// </summary>
    /// <returns>생성된 bullet</returns>
    public GameObject CreateBullet()
    {
        return bulletPool.NewItem();
    }

    /// <summary>
    /// effect 생성
    /// </summary>
    /// <returns>생성된 effect</returns>
    public GameObject CreateEffect()
    {
        return effectPool.NewItem();
    }
    #endregion

    #region deleteObject

    /// <summary>
    /// weapon object 삭제(회수)
    /// </summary>
    /// <param name="obj"></param>
    public void DeleteWeapon(GameObject obj)
    {
        weaponPool.RemoveItem(obj);
    }

    /// <summary>
    /// bullet object 삭제(회수)
    /// </summary>
    /// <param name="obj"></param>
    public void DeleteBullet(GameObject obj)
    {
        bulletPool.RemoveItem(obj);
    }

    /// <summary>
    /// effect object 삭제(회수)
    /// </summary>
    /// <param name="obj"></param>
    public void DeleteEffect(GameObject obj)
    {
        effectPool.RemoveItem(obj);
    }

    #endregion

    private void OnApplicationQuit()
    {
        //메모리 풀을 비웁니다.
        bulletPool.Dispose();
    }
    #endregion

}
