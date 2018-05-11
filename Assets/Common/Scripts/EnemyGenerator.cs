using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviourSingleton<EnemyGenerator> {

    public Sprite[] sprites;
    public ObjectPool objectPool;
    public GameObject alertObj;

    public void Generate(Vector3 _position)
    {
        GameObject obj = Instantiate(alertObj,_position,Quaternion.identity,this.transform);
        obj.AddComponent<Alert>();
        obj.GetComponent<Alert>().sprite = null;
        obj.GetComponent<Alert>().Init(CallBack);
        obj.GetComponent<Alert>().Active();
    }

    void CallBack(Vector3 _position)
    {
        GameObject obj = objectPool.GetPooledObject();
        obj.transform.position = _position;
        obj.GetComponent<Enemy>().Init(sprites[0]);
        obj.GetComponent<BoxCollider2D>().size = sprites[0].bounds.size;
    }
}
