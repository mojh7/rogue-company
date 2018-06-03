using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp : MonoBehaviour {

    public EnemyData EnemyData;
    public GameObject GameObject;
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.A))
        {
            EnemyManager.Instance.GenerateEnemyFromData(EnemyData, GameObject);
        }
        
	}
}
