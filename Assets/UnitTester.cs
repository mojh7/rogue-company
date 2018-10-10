using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTester : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ItemManager.Instance.CallItemBox(Vector2.zero, RoomType.BOSS);
	}
 
}
