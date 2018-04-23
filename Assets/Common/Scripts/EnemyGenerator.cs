using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour {
    private static EnemyGenerator instance;

    public Sprite[] sprites;
    public ObjectPool objectPool;
    GameObject enemyObj;
    
    public static EnemyGenerator GetInstance()
    {
        if(instance == null)
        {
            instance = Object.FindObjectOfType<EnemyGenerator>();
        }
        return instance;
    }

    public void Generate(Vector3 _position)
    {
        enemyObj = objectPool.GetPooledObject();
        enemyObj.transform.position = _position;
        enemyObj.GetComponent<SpriteRenderer>().sprite = sprites[0];
        enemyObj.GetComponent<BoxCollider2D>().size = sprites[0].bounds.size;
    }

}
