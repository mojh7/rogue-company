using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 일단 무식하게라도 ai를 만들기☆

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Sunglasses : MonoBehaviour {

    #region variables
    public float hp = 10.0f;            // 선글라스 hp
    public float mSpeed = 2.0f;         // 선글라스 이동속도
    public float realize = 6.0f;        // 선글라스 인식 범위
    public float attackRan = 3.0f;      // 선글라스 공격 범위 -> 0524 기획서 명시x
    // bool recall = false;                // 선글라스 소환 여부 x

    // Component
    [HideInInspector]
    public Rigidbody2D rd;
    [HideInInspector]
    public Collider2D coll;
    [HideInInspector]

    // Object
    public GameObject player;

    // Scripts
    MovingPattern mp = new MovingPattern();
    #endregion

    void Awake()
    {
        rd = this.GetComponent<Rigidbody2D>();
        coll = this.GetComponent<Collider2D>();
        player = GameObject.Find("Player");
    }

    void FixedUpdate()
    {
        // 1. 카메라가 몬스터 쪽에 위치해 있으면 setActive
        // 2. setActive 되어 잇으면 몬스터 회전
        // 3. 플레이어와 거리차가 realize보다 같거나 작아지면 플레이어를 인식
        // 4. 사정거리(-0.5f)가 다가올 때 까지 추적
        // 5. 플레이어를 향해 침뱉기 공격
        // 5.5. 사정거리에 벗어나면 다시 추적하고 5번 반복
        // 6. hp가 0이 되면 사라진다(exExplos는 false);
    }

    public void Die(bool isExplos)
    {
        if (hp != 0)
            return;
        if (isExplos)
        {
            Debug.Log("터진다");
        }
        else
        {
            Debug.Log("사라진다");
        }
    }
}
