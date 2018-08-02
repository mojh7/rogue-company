using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviourSingleton<EnemyManager>
{

    private Sprite sprite;
    [SerializeField]
    private EnemyData[] enemyDatas;
    public ObjectPool objectPool;
    private List<Enemy> enemyList;

    // 0516 모장현
    private int aliveEnemyTotal;

    private void Start()
    {
        enemyList = new List<Enemy>();
        aliveEnemyTotal = 0;
    }

    #region 바꿔야되는것들
    public void Generate(Vector2 position, EnemyData enemyData)
    {
        GameObject obj = ResourceManager.Instance.objectPool.GetPooledObject();
        obj.transform.position = position;
        obj.AddComponent<Alert>();
        obj.GetComponent<Alert>().Init(CallBack, enemyData, 0, 0);
        obj.GetComponent<Alert>().Active();
    }

    public void Generate(Vector3 _position)
    {
        GameObject obj = ResourceManager.Instance.objectPool.GetPooledObject();
        obj.transform.position = _position;
        obj.AddComponent<Alert>();
        obj.GetComponent<Alert>().Init(CallBack, enemyDatas[0], 0, 0);
        obj.GetComponent<Alert>().Active();
    }

    public Sprite GetBossSprite()
    {
        sprite = enemyDatas[0].Sprite;
        return sprite;
    }

    GameObject SpawnEnemy(Vector3 position)
    {
        GameObject obj = objectPool.GetPooledObject();
        obj.transform.position = position;
        obj.transform.localScale = new Vector3(1, 1, 0);

        aliveEnemyTotal += 1;

        return obj;
    }

    public void SpawnBoss(int _floor, Vector2 position)
    {
        BossEnemy enemy;
        GameObject obj = SpawnEnemy(position);

        enemy = obj.AddComponent<BossEnemy>();
        enemy.Init(enemyDatas[0]);
        enemyList.Add(enemy);

        UIManager.Instance.bossHPUI.Toggle();
        UIManager.Instance.bossHPUI.SetHpBar(enemy.GetHP());
    }

    void CallBack(Vector3 position, object temporary, float amount)
    {
        Enemy enemy;
        GameObject obj = SpawnEnemy(position);

        enemy = obj.AddComponent<Enemy>();
        enemy.Init(temporary as EnemyData);
        enemyList.Add(enemy);
    }
    #endregion

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
    }

    // 0516 모장현
    public int GetAliveEnemyTotal()
    {
        return aliveEnemyTotal;
    }
}
