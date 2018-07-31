using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponTest : MonoBehaviour
{
    [SerializeField]
    private EnemyData enemyData;
    [SerializeField]
    private GameObject enemyObj;
    [SerializeField]
    private Enemy enemy;
    [SerializeField]
    private WeaponManager weaponManager;

	// Use this for initialization
	void Start ()
    {
        enemy = enemyObj.AddComponent<Enemy>();
        enemy.Init(enemyData);
        //enemyList.Add(enemy);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
