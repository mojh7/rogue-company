using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviourSingleton<EnemyGenerator> {
    private static EnemyGenerator instance;

    public Sprite[] sprites;
    public ObjectPool objectPool;
    GameObject enemyObj;
    
    public void Generate(Vector3 _position)
    {
        enemyObj = new GameObject();
        enemyObj.AddComponent<AlertIndicator>();
    }

    void Spawn(GameObject _enemyObj, Vector2 _position)
    {
        _enemyObj = objectPool.GetPooledObject();
        _enemyObj.transform.position = _position;
        _enemyObj.GetComponent<SpriteRenderer>().sprite = sprites[0];
        _enemyObj.GetComponent<BoxCollider2D>().size = sprites[0].bounds.size;
    }

}
