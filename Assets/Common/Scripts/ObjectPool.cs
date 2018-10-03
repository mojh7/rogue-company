using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{ // 오브젝트 풀링을 위한 클래스

    public GameObject pooledObject; // 오브젝트
    public int pooledAmount = 20; // 오브젝트 수
    public bool willGrow = true; // 오브젝트 제한 여부

    List<GameObject> pooledObjects;

    // Use this for initialization
    void Awake () {
        pooledObjects = new List<GameObject>();
        for(int i = 0; i < pooledAmount; i++)
        {
            GameObject obj = (GameObject)Instantiate(pooledObject);
            //obj.hideFlags = HideFlags.HideInHierarchy;
            obj.SetActive(false);
            obj.transform.parent = transform;
            pooledObjects.Add(obj);
        }
	}

    public void Deactivation()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (pooledObjects[i].activeInHierarchy)
            {
                pooledObjects[i].SetActive(false);
            }
        }
    }
    
    public GameObject GetPooledObject() // Pool에서 오브젝트 가져오기
    {
        for(int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                pooledObjects[i].SetActive(true);
                return pooledObjects[i];
            }
        }

        if (willGrow) // 오브젝트 더 생성.
        {
            GameObject obj = (GameObject)Instantiate(pooledObject);
            obj.transform.parent = transform;
            pooledObjects.Add(obj);
            return obj;
        }

        return null;
    }
}
