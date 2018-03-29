using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player가 방향키를 사용해서 이동하는 테스트
// -> 네비게이션 테스트 스크립트

public class PlayerController : MonoBehaviour {

    public float pSpeed = 3f;

	void Start () {
		
	}
	
	void Update () {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(x, y, 0);
        dir.Normalize();

        transform.position += dir * pSpeed * Time.deltaTime;
    }
}
