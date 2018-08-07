using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviourSingleton<EnemyManager>
{
    [System.Serializable]
    struct FloorData
    {
        public EnemyData[] enemyDatas;
        public EnemyData[] bossEnemyDatas;
    }

    private Sprite sprite;
    [SerializeField]
    private FloorData[] floorDatas;
    public ObjectPool objectPool;
    private List<Enemy> enemyList;
    private List<CircleCollider2D> enemyColliderList;

    // 0516 모장현
    private int aliveEnemyTotal;

    private void Start()
    {
        enemyList = new List<Enemy>();
        enemyColliderList = new List<CircleCollider2D>();
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
        obj.GetComponent<Alert>().Init(CallBack, GetEnemy(false), 0, 0);
        obj.GetComponent<Alert>().Active();
    }

    public Sprite GetBossSprite()
    {
        sprite = GetEnemy(true).Sprite;
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
        enemy.Init(GetEnemy(true));
        enemyList.Add(enemy);
        enemyColliderList.Add(enemy.GetCircleCollider2D());

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
        enemyColliderList.Add(enemy.GetCircleCollider2D());
    }
    #endregion

    EnemyData GetEnemy(bool isBoss)
    {
        int floor = InGameManager.Instance.GetFloor();
        if (floor >= floorDatas.Length)
        {
            floor = floorDatas.Length - 1;
        }
        if (isBoss)
        {
            int rand = Random.Range(0, floorDatas[floor].bossEnemyDatas.Length);
            rand = 1;
            return floorDatas[floor].bossEnemyDatas[rand];
        }
        else
        {
            int rand = Random.Range(0, floorDatas[floor].enemyDatas.Length);

            return floorDatas[floor].enemyDatas[rand];
        }
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

    public List<CircleCollider2D> GetEnemyColliderList
    {
        get
        {
            if (enemyColliderList == null)
                return null;
            return enemyColliderList;
        }
    }

    public void DeleteEnemy(Enemy _enemy)
    {
        if (enemyList == null)
            return;
        aliveEnemyTotal -= 1;
        enemyColliderList.Remove(_enemy.GetCircleCollider2D());
        enemyList.Remove(_enemy);
    }

    // 0516 모장현
    public int GetAliveEnemyTotal()
    {
        return aliveEnemyTotal;
    }
}
