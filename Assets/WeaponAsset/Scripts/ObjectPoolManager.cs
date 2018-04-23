using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 일단 가지고 있던 메모리풀 쓰고 있는 중.....................................


public enum ObjPoolType { Bullet, Effect }
public class ObjectPoolManager : MonoBehaviour {

    #region variables
    
    public GameObject bulletObj;
    public int bulletNumMax;
    public MemoryPool bulletPool;

    // 싱글톤
    private static ObjectPoolManager instance = null;

    #endregion

    #region getter
    public static ObjectPoolManager Instance { get { return instance; } }
    #endregion

    
    void Start () {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);

        // 오브젝트 풀 초기화
        // 총알 오브젝트풀 초기화
        bulletPool = new MemoryPool(bulletObj, bulletNumMax);
	}

    #region function
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
