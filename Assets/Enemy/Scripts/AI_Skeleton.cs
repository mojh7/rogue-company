using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class AI_Skeleton : MonoBehaviour {

    #region variables
    [SerializeField]
    float hp = 0.0f;             // hp
    float mSpeed = 0.0f;         // 이동속도
    float realize = 0.0f;        // 인식 범위
    [SerializeField]
    float attackRan = 0.0f;      // 공격 범위
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
    public SkeletonState state;

    // 상태머신 -> 애니메이션
    public enum SkeletonState
    {
        Idle,           // 인식 전
        Track,          // 추적
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
            case SkeletonState.Idle:
                mp.Stop();
                break;
            case SkeletonState.Track:
                mp.Track(attackRan, mSpeed);
                break;
            case SkeletonState.Attack:
                Attack();
                break;
            case SkeletonState.BeAttacked:
                BeAttack();
                break;
            case SkeletonState.Die:
                Die();
                break;
        }
        if (mp.moveDif > realize)
        {
            state = SkeletonState.Idle;
        }
        else
        {
            mp.isTurn = true;
            state = SkeletonState.Track;
            if (hp <= 0)
            {
                state = SkeletonState.Die;
            }
        }
    }

    #region fun
    private void Init()
    {
        hp = 11;
        mSpeed = 2.5f;
        realize = 5;
        attackRan = 1.5f;
        coolTime = 0.7f;
        attackSpeed = 1.5f;
        state = SkeletonState.Idle;
        coll.isTrigger = true;
    }
    private void Attack()
    {
        // 할퀴기 공격
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
        state = SkeletonState.BeAttacked;
    }
    #endregion
}
