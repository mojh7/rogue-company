using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviourSingleton<EnemyManager> {

    public Sprite[] sprites;
    public ObjectPool objectPool;
    public GameObject alertObj;

    private List<Enemy> enemyList;
    // 0516 모장현
    private int aliveEnemyTotal;


    private void Awake()
    {
        enemyList = new List<Enemy>();
        aliveEnemyTotal = 0;
    }

    public void Generate(Vector3 _position)
    {
        GameObject obj = Instantiate(alertObj,_position,Quaternion.identity,this.transform);
        obj.AddComponent<Alert>();
        obj.GetComponent<Alert>().sprite = null;
        obj.GetComponent<Alert>().Init(CallBack);
        obj.GetComponent<Alert>().Active();
    }

    public Sprite GetBossSprite(int _floor)
    {
        Sprite sprite = sprites[Mathf.Clamp(_floor, 0, sprites.Length - 1)];
        return sprite;
    }

    public void SpawnBoss(int _floor,Vector2 _position)
    {
        Enemy enemy;
        GameObject obj = objectPool.GetPooledObject();
        Sprite sprite = sprites[Mathf.Clamp(_floor, 0, sprites.Length - 1)];
        obj.transform.position = _position;
        obj.transform.localScale = new Vector3(2, 2, 0);
        enemy = obj.GetComponent<Enemy>();
        enemy.Init(sprite);
        enemyList.Add(enemy);
        obj.GetComponent<BoxCollider2D>().size = sprite.bounds.size;
        aliveEnemyTotal += 1;
    }

    void CallBack(Vector3 _position)
    {
        Enemy enemy;
        GameObject obj = objectPool.GetPooledObject();
        Sprite sprite = sprites[Random.Range(0, sprites.Length)];
        obj.transform.position = _position;
        enemy = obj.GetComponent<Enemy>();
        enemy.Init(sprite);
        enemyList.Add(enemy);
        obj.GetComponent<BoxCollider2D>().size = sprite.bounds.size;
        aliveEnemyTotal += 1;
    }

    public List<Enemy> GetEnemyList
    {
        get
        {
            if (enemyList == null)
                return null;
            return enemyList;
        }
    }

    public void DeleteEnemy(Enemy _enemy)
    {
        if (enemyList == null)
            return;
        aliveEnemyTotal -= 1;
        enemyList.Remove(_enemy);
        //enemyTransformList.Remove(_enemy.transform);
    }

    // 0516 모장현
    public int GetAliveEnemyTotal()
    {
        return aliveEnemyTotal;
    }
}
