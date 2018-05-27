using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 선글라스 사원의 눈물공격 스크립트
public class Sunglasses_Tear : MonoBehaviour {

    public float speed = 1.4f;

    public GameObject player;

    float currentTime = 0;

    Vector3 dir;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        //dir = transform.up;
    }

    private void FixedUpdate()
    {
        // 0528테스트 - 로컬기준 오른쪽을 향해서 눈물공격 발사
        // 수정해야할것 = 처음 active true될때의 로컬 오른쪽 방향 저장 후 그쪽으로만 이동되게 하기
        currentTime += Time.deltaTime;
        if (currentTime < 1.5f)
            transform.position += transform.up * speed * Time.deltaTime;
        else
        {
            currentTime = 0;
            this.gameObject.SetActive(false);
        }
        //transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject == player)
        {
            Debug.Log("Player 충돌");
            gameObject.SetActive(false);
        }
        else if (other.gameObject.name == "sunglasses")
        {
            Debug.Log("몬스터랑 충돌");
        }
        else
        {
            Debug.Log("다른물체 충돌");
        }
    }
}
