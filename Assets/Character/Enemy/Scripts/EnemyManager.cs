﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviourSingleton<EnemyManager> {

    private Sprite sprite;
    [SerializeField]
    private EnemyData enemyData;
    public ObjectPool objectPool;
    private List<Enemy> enemyList;
    GameObject alertObj;

    // 0516 모장현
    private int aliveEnemyTotal;

    private void Start()
    {
        enemyList = new List<Enemy>();
        aliveEnemyTotal = 0;
        alertObj = ResourceManager.Instance.ObjectPrefabs;
    }

    #region 바꿔야되는것들
    public void Generate(Vector3 _position)
    {
        GameObject obj = Instantiate(alertObj,_position,Quaternion.identity,this.transform);
        obj.AddComponent<Alert>();
        obj.GetComponent<Alert>().Init(CallBack);
        obj.GetComponent<Alert>().Active();
    }

    public Sprite GetBossSprite()
    {
        sprite = enemyData.Sprite;
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

    public void SpawnBoss(int _floor,Vector2 position)
    {
        BossEnemy enemy;
        GameObject obj = SpawnEnemy(position);

        enemy = obj.AddComponent<BossEnemy>();
        enemy.Init(enemyData);
        enemyList.Add(enemy);

        UIManager.Instance.bossHPUI.Toggle();
        UIManager.Instance.bossHPUI.SetHpBar(enemy.GetHP());
    }

    void CallBack(Vector3 position)
    {
        Enemy enemy;
        GameObject obj = SpawnEnemy(position);

        enemy = obj.AddComponent<Enemy>();
        enemy.Init(enemyData);
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