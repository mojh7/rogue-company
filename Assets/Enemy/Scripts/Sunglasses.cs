using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 일단 무식하게라도 ai를 만들기☆
// Enemy 의 공격하는 임팩트(?) pool 만들어 쓰자.
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Sunglasses : MonoBehaviour {

    #region varies
    [SerializeField]
    float hp = 0.0f;            // 선글라스 hp
    float mSpeed = 0.0f;         // 선글라스 이동속도
    float realize = 0.0f;        // 선글라스 인식 범위
    [SerializeField]
    float attackRan = 0.0f;      // 선글라스 공격 범위 -> 0524 기획서 명시x
    // bool recall = false;                // 선글라스 소환 여부 x
    [SerializeField]
    bool isActive = false;              // 개체의 숨결
    bool isRotate = false;              // 개체 회전
    float moveDif;                      // 거리차
    // Astar
    [SerializeField]
    bool isPath = false;
    Vector3[] path;
    private GameObject point;
    public LayerMask unwalkableMask;
    int targetIndex;
    // object pool
    int TEARS_POOL_SIZE = 5;

    public SunglassedPatt state;

    // Component
    [HideInInspector]
    public Rigidbody2D rd;
    [HideInInspector]
    public Collider2D coll;

    // Object
    public GameObject player;
    public GameObject tear;
    //GameObject[] tearspool;

    // Scripts
    AI_MovingPattern mp = new AI_MovingPattern();
    #endregion

    // 상태머신 -> 애니메이션
    public enum SunglassedPatt
    {
        Idle,           // 인식 전
        Track,          // 추격
        Attack,         // (정지상태로) 공격
        BeAttacked,     // 피격
        Die,            // 죽음
    }

    void Awake()
    {
        rd = this.GetComponent<Rigidbody2D>();
        coll = this.GetComponent<Collider2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        if (this.enabled == true)
        {
            isActive = true;
        }
        Init();
    }

    void FixedUpdate()
    {
        // 1. 카메라가 몬스터 쪽에 위치해 있으면 setActive(Object Pool에서 건들기)
        // 2. setActive 되어 있으면 몬스터 회전
        // 3. 플레이어와 거리차가 realize보다 같거나 작아지면 플레이어를 인식
        // 4. 사정거리가 다가올 때 까지 추적
        // 5. 플레이어를 향해 침뱉기 공격
        // 5.5. 사정거리(+0.2f)에 벗어나면 다시 추적하고 5번 반복
        // 6. hp가 0이 되면 사라진다(exExplos는 false);
        if (isRotate)
        {
            ObjectRotate();
        }
        moveDif = Vector3.Distance(transform.position, player.transform.position);
        // 얘는 상태머신을 위한 스위치.
        switch (state)
        {
            case SunglassedPatt.Idle:
                isPath = false;
                Idle();
                break;
            case SunglassedPatt.Track:
                isPath = true;
                if (isActive)
                    Track();
                break;
            case SunglassedPatt.Attack:
                isPath = false;
                if (isActive)
                    Attack();
                break;
            case SunglassedPatt.BeAttacked:
                isPath = false;
                if (isActive)
                    BeAttacked();
                break;
            case SunglassedPatt.Die:
                isPath = false;
                Die(false);
                break;
        }

        //if (Input.GetButtonDown("Fire1"))
        //{
        //    Debug.Log("Fire1 Test");
        //}
    }

    #region fun

    private void Init()
    {
        state = SunglassedPatt.Idle;
        hp = 10.0f;
        mSpeed = 2.0f;
        realize = 6.0f;
        attackRan = 3.0f;
        isRotate = true;
        tear.SetActive(false);
        //tearspool = new GameObject[TEARS_POOL_SIZE];
        //for (int i = 0; i < TEARS_POOL_SIZE; i++)
        //{
        //    tearspool[i] = Instantiate(tears);
        //    tearspool[i].transform.parent = transform;
        //    tearspool[i].SetActive(false);
        //}
    }

    private void ObjectRotate()
    {
        if (transform.position.x < player.transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    private void Idle()
    {
        // 플레이어와 거리차가 realize보다 같거나 작아지면 플레이어를 인식
        if (moveDif <= realize)
        {
            state = SunglassedPatt.Track;
        }
        else
        {
            return;
        }
    }

    private void Track()
    {
        // 사정거리가 다가올 때 까지 추적
        if (moveDif > attackRan)
        {
            PathRequestManager.RequestPath(new PathRequest(transform.position, player.transform.position, OnPathFound));
        }
        else
        {
            // path 노드 초기화
            state = SunglassedPatt.Attack;
        }
        if (!isPath)
            isPath = true;
    }

    // 0528Text
    public float currentTime = 0;
    private void Attack()
    {
        // 플레이어가 사정거리 + 0.5 에서 멀어지면 코루틴 스탑
        if (moveDif > attackRan + 0.5f)
        {
            // 다시 추적
            //StopCoroutine("AttackTears");
            currentTime = 0;
            state = SunglassedPatt.Track;
        }
        else
        {
            // 코루틴 시작
            //StartCoroutine("AttackTears");
            currentTime += Time.deltaTime;
            // 수정해야할 것 : 공격하는 동안은 시간 이동 xx
            if (currentTime < 2.5f)
            {
                //tear = Instantiate(tears);
                //tear.transform.parent = this.transform;
                //tear.transform.position = this.transform.position;
                if (tear.activeSelf == false)
                {
                    tear.transform.position = this.transform.position;
                    tear.SetActive(true);
                }
            }
            else
            {
                currentTime = 0;
            }
        }
    }

    // 공격하는 코루틴
    //IEnumerator AttackTears()
    //{
    //    // 공격 유지 시간
    //    // 0528 03 : 17 Q. 왜.. 오브젝트 풀이 전부가 active될까 ㅎ??..난쓰레기야..
    //    // -> 그냥 엎자 코루틴 사용 오브젝트풀 사용 XXX
    //    for (int i = 0; i < TEARS_POOL_SIZE; i++)
    //    {
    //        if (tearspool[i].activeSelf == false)
    //        {
    //            Debug.Log(i);
    //            GameObject tear = tearspool[i];
    //            tear.SetActive(true);
    //            tear.transform.position = this.transform.position;
    //            break;
    //        }
    //    }
    //    yield return new WaitForSeconds(3f);
    //}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            // 플레이어의 공격력에 따라 hp가 깎임.
            // 애니메이션은 피격효과
            if (hp <= 0)
            {
                state = SunglassedPatt.Die;
            }
            else
            {
                state = SunglassedPatt.BeAttacked;
            }
        }
        else
        {
            Debug.Log("TriggerTest");
        }
    }

    private void BeAttacked()
    {
        // 피격 애니메이션
        //throw new NotImplementedException();
        // 0528 Test용으로 hp-1씩 깎기
        hp -= 1;
        if (hp <= 0)
            state = SunglassedPatt.Die;
        else
            state = SunglassedPatt.Attack;
    }

    public void Die(bool isExplos)
    {
        if (hp > 0)
        {
            state = SunglassedPatt.Attack;
            return;
        }
        if (isExplos)
        {
            Debug.Log("터진다");
        }
        else
        {
            Debug.Log("사라진다");
            isActive = false;
            isRotate = false;
            DestroyObject(this.gameObject, 0.5f);
        }
    }
    #endregion

    #region astar
    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            targetIndex = 0;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }
    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];

        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                Debug.Log(player.transform.position);
                targetIndex++;
                Debug.Log(path.Length);
                if (targetIndex >= (path.Length + 1))
                {
                    yield break;
                }
                //currentWaypoint = path[targetIndex];
            }
            transform.position = Vector3.MoveTowards
                (transform.position, currentWaypoint, mSpeed * Time.deltaTime);
            yield return null;
        }
    }
    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
    #endregion
}
