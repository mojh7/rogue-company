using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Sunglasses : MonoBehaviour {

    #region variables
    [SerializeField]
    float hp = 0.0f;            // 선글라스 hp
    float mSpeed = 0.0f;         // 선글라스 이동속도
    float realize = 0.0f;        // 선글라스 인식 범위
    [SerializeField]
    float attackRan = 0.0f;      // 선글라스 공격 범위
    float coolTime = 0;          // 공격 쿨타임
    float attackSpeed = 0;       // 공격속도
    [SerializeField]
    bool isActive = false;              // 개체의 숨결

    // Component
    [HideInInspector]
    public Rigidbody2D rd;
    [HideInInspector]
    public Collider2D coll;
    AI_MovingPattern mp = new AI_MovingPattern();

    // 상태머신
    public SunglassedPatt state;

    // 상태머신 -> 애니메이션
    public enum SunglassedPatt
    {
        Idle,           // 인식 전
        Attack,         // (정지상태로) 공격
        BeAttacked,     // 피격
        Die,            // 죽음
    }
    #endregion

    void Awake ()
    {
        rd = this.GetComponent<Rigidbody2D>();
        coll = this.GetComponent<Collider2D>();
        mp = GetComponent<AI_MovingPattern>();
        Init();
    }

	void FixedUpdate ()
    {
        switch (state)
        {
            case SunglassedPatt.Idle:
                mp.Stop();
                break;
            case SunglassedPatt.Attack:
                mp.isTurn = true;
                Attack();
                break;
            case SunglassedPatt.BeAttacked:
                BeAttack();
                break;
            case SunglassedPatt.Die:
                Die();
                break;
        }
        if (mp.moveDif > realize)
        {
            state = SunglassedPatt.Idle;
        }
        else
        {
            state = SunglassedPatt.Attack;
            if (hp <= 0)
            {
                state = SunglassedPatt.Die;
            } 
        }
    }

    #region fun
    private void Init()
    {
        hp = 10;
        mSpeed = 2.0f;
        realize = 6;
        attackRan = 6;
        coolTime = 0.3f;
        attackSpeed = 1.5f;
        state = SunglassedPatt.Idle;
        coll.isTrigger = true;
    }

    private void Attack()
    {
        // 눈물공격
        Debug.Log("공격한당");
    }
    
    private void BeAttack()
    {
        // 테스트 = 1씩 달기
        hp -= 1;
    }
    
    private void Die()
    {
        Debug.Log("사라진당");
        mp.Stop();
        Destroy(this.gameObject, 1.5f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 테스트용 일단 맞으면 BeAttack
        state = SunglassedPatt.BeAttacked;
    }
    #endregion
}
