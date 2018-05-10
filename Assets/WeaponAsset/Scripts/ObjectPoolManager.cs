using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 일단 가지고 있던 메모리풀 쓰고 있는 중... - 장현

public enum ObjPoolType { Bullet, Effect }
public class ObjectPoolManager : MonoBehaviourSingleton<ObjectPoolManager> {

    #region variables
    
    public GameObject bulletObj;
    public int bulletNumMax;
    public MemoryPool bulletPool;

    #endregion

    #region getter

    #endregion

    
    void Start () {
        // 오브젝트 풀 초기화
        // 총알 오브젝트풀 초기화
        bulletPool = new MemoryPool(bulletObj, bulletNumMax);
	}

    #region function
    // 나중에 오브젝트 풀 바껴도 CreateObj, DeleteObj 함수 내부만 바꾸면 되서 함수 새로 만듬.
    
    /// <summary>
    /// GameObject 생성
    /// </summary>
    /// <param name="objPoolType">생성할 오브젝트 타입</param>
    /// <returns>생성된 게임 오브젝트</returns>
    public GameObject CreateObj(ObjPoolType objPoolType)
    {
        switch (objPoolType)
        {
            case ObjPoolType.Bullet:
                return bulletPool.NewItem();
            default:
                return null;
        }
    }
    ///
    // Obj 삭제(회수)
    public void DeleteObj(ObjPoolType objPoolType, GameObject obj)
    {
        switch(objPoolType)
        {
            case ObjPoolType.Bullet:
                bulletPool.RemoveItem(obj);
                break;
            default:
                break;
        }
    }

    private void OnApplicationQuit()
    {
        //메모리 풀을 비웁니다.
        bulletPool.Dispose();
    }
    #endregion

}
