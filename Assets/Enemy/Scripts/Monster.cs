using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 몬스터 기본 Setting
// ObjectID, Scale, Level, Hp, mSpeed, realize, recall, dieEffect

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]

public class Monster : MonoBehaviour {

    //#region variables
    ///* 위 내용들은 내가 구분할 수 있도록 해둔 것 의미xxx
    //objectID;               // 객체 ID 이걸 코드에서 나누면 의미가 있나?
    //sclae = 1.0f;           // 객체 크기(디폴트 : 1)
    //level;                  // 몬스터 레밸
    //mSpeed;                 // 이동 속도
    //realize;                // 인식 범위
    //recall;                // 소환 여부
    //*/

    //public float hp;                   // 몬스터 hp

    //[HideInInspector]
    //public Rigidbody2D rd;
    //[HideInInspector]
    //public Collider2D coll;
    //#endregion

    //// 생성자
    ////public Monster(float hp)
    ////{
    ////    this.hp = hp;
    ////}

    //public void Awake()
    //{
    //    rd = this.GetComponent<Rigidbody2D>();
    //    coll = this.GetComponent<Collider2D>();
    //}

    //public void Die(bool isExplos)
    //{
    //    if (hp != 0)
    //        return;
    //    if (isExplos)
    //    {
    //        Debug.Log("터진다");
    //    }
    //    else
    //    {
    //        Debug.Log("사라진다");
    //    }
    //}
}
