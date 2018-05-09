using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 이동패턴 구현
public class MovingPattern : MonoBehaviour
{
    // 1. 이동 : 1) 정지 2) 좌우 움직임 3)추적 4) 돌진 5) 시계방향 이동 6) 역추적

    [SerializeField]
    public GameObject player;
    public float moveSpeed = 1.5f;
    Vector3[] path;

    // 원형 이동
    private float radius = 3;
    private float runningTime = 0;
    private Vector3 newPos = new Vector3();

    // 돌진
    [SerializeField] [HideInInspector]
    private GameObject point;
    public LayerMask unwalkableMask;

    int targetIndex;                            // Astar path index
    [SerializeField]
    private float currentTime;                          // 시간
    float moveDif;                              // 거리차

    public static MovingPattern instance;       // 인스턴스
    public bool isPath = false;                 // Astar bool
    public bool isTurn = true;                 // 몬스터 y축 회전 bool

    public float test_StopTr = 3f;

    // 테스트용 상태머신
    public enum MovingPatt
    {
        stop,           // 정지 상태
        moveLR,         // 좌우 움직임
        track,          // 추적
        dash,           // 돌진
        moveCir,        // 시계방향 이동
        untrack,        //역추적 
    }

    public MovingPatt state;

    private void Awake()
    {
        player = GameObject.Find("Player");
        point = transform.Find("point").gameObject;
        state = MovingPatt.stop;
        Debug.Log(point);
        if (instance == null)
        {
            instance = this;
        }
    }

    void Update()
    {
        // 거리차
        moveDif = Vector3.Distance(transform.position, player.transform.position);
        Debug.Log("거리차 : " + moveDif);

        switch (state)
        {
            // 멈춤
            case MovingPatt.stop:
                isPath = false;
                //isTurn = false;
                Stop();
                break;
            // 추적
            case MovingPatt.track:
                isPath = true;
                Track();
                break;
            // 역추적
            case MovingPatt.untrack:
                Untrack();
                break;
            // 대쉬
            case MovingPatt.dash:
                Dash();
                break;
            // 좌우 왔다갔다
            case MovingPatt.moveLR:
                MoveLR();
                break;
            // 시계방향으로 이동
            case MovingPatt.moveCir:
                MoveCir();
                break;
        }

        // 몬스터 회전.  
        if (isTurn)
        {
            // 플레이어의 x축보다 작으면 y축으로 90도 회전 크면 y축으로 180도 회전
            if (transform.position.x < player.transform.position.x)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }

        #region test
        // Player를 원점으로 반지름 3만큼 해서 1.5f스피드로 
        /*
        runningTime += Time.deltaTime * 0.5f;
        float x = player.transform.position.x + radius * Mathf.Cos(runningTime);
        float y = player.transform.position.y + radius * Mathf.Sin(runningTime);
        newPos = new Vector3(x, y, 0);
        this.transform.position = newPos;*/

        // 일단 원하는 위치까지 이동하는 스크립트
        /*
        Vector3 dir = (new Vector3(0, 3, 0) - transform.position).normalized;
        if (transform.position.y < 3)
        {
            this.transform.Translate(dir * moveSpeed * Time.deltaTime);
        }
        else
        {
            return;
        }
        */
        // player와 거리차
        //
        //if (moveDif >= 2)
        //{
        //    point.gameObject.SetActive(true);
        //    // 거리차가 어느정도 오면 point setActive false
        //    // 매개변수로 스피드를 넣어줄까?? 아니면 그냥 *2를 하고 끝나면 다시 /2하기 
        //}
        #endregion
    }

    #region moveFun
    private void Stop()
    {

    }

    private void Track()
    {
        if (!isPath)
        {
            isPath = true;
        }
        else
        {
            PathRequestManager.RequestPath(new PathRequest
                (transform.position, player.transform.position, OnPathFound));
            if (moveDif < test_StopTr)
            {
                isPath = false;
                state = MovingPatt.moveCir;
                return;
            }
        }
    }

    private void Untrack()
    {

    }

    private void Dash()
    {
        // 만일 장애물(Mask사용)이 없고, 플레이어의 거리차가 2이하이면 돌진.
        int layerMask = (-1) - ((1 << unwalkableMask));
        RaycastHit hit;
        bool isMask = Physics.Raycast(transform.position,
            transform.TransformDirection(Vector3.right), out hit, Mathf.Infinity, layerMask);
        // 추가, 수정할 것 : mask

        bool isDash = moveDif < 5;
        bool isMove = moveDif > test_StopTr;
        if (isDash && isMove)
        {
            point.gameObject.SetActive(true);
            Vector3 dir = (new Vector3(0, 3, 0) - transform.position).normalized;
            this.transform.Translate(dir * moveSpeed * 2 * Time.deltaTime);
        }
        if (!isMove)
        {
            point.gameObject.SetActive(false);
            state = MovingPatt.stop;
        }
        Debug.Log(isDash);
    }

    int ran;
    int LRpos = 0;        // 왼쪽 오른쪽
    int UDpos = 0;        // 아래 위래
    private void MoveLR()
    {
        currentTime += Time.deltaTime;
        if (currentTime > 2f)
        {
            ran = RandomInt(0, 1);
            Debug.Log(ran);
            if (ran == 0)
            {
                if (LRpos == 0)
                {
                    Vector3 dir = (new Vector3(transform.position.x + 2, transform.position.y, transform.position.z) - transform.position).normalized;
                    if (transform.position.x < dir.x)
                    {
                        this.transform.Translate(dir * moveSpeed * Time.deltaTime);
                    }
                    else
                    {
                        currentTime = 0;
                    }
                    LRpos = 1;
                    print("오른쪽으로 이동이욤");
                }
                else if (LRpos == 1)
                {
                    Vector3 dir = (new Vector3(transform.position.x - 2, transform.position.y, transform.position.z) - transform.position).normalized;
                    Debug.Log(dir);
                    LRpos = 0;
                    print("왼쪽으로 이동이욤");
                }
            }
            //else
            //{
            //    if (UDpos == 0)
            //    {
            //        Vector3 dir = (new Vector3(transform.position.x, transform.position.y + 2, transform.position.z) - transform.position).normalized;
            //        if (transform.position.y > dir.y)
            //        {
            //            this.transform.Translate(dir * moveSpeed * Time.deltaTime);
            //        }
            //        else
            //        {
            //            currentTime = 0;
            //        }
            //        UDpos = 1;
            //        print("위쪽으로 이동이욤");
            //    }
            //    else if (LRpos == 1)
            //    {
            //        Vector3 dir = (new Vector3(transform.position.x, transform.position.y - 2, transform.position.z) - transform.position).normalized;
            //        if (transform.position.y < dir.y)
            //        {
            //            this.transform.Translate(dir * moveSpeed * Time.deltaTime);
            //        }
            //        else
            //        {
            //            currentTime = 0;
            //        }
            //        UDpos = 0;
            //        print("아래쪽으로 이동이욤");
            //    }
            //}
        }
    }

    private void MoveCir()
    {
        // Player를 원점으로 반지름 3만큼 해서 1.5f스피드로 
        runningTime += Time.deltaTime * 0.5f;
        float x = player.transform.position.x + radius * Mathf.Cos(runningTime);
        float y = player.transform.position.y + radius * Mathf.Sin(runningTime);
        newPos = new Vector3(x, y, 0);
        this.transform.position = newPos;
        // 추가할 것 : 시작점.
    }

    private int RandomInt(int min, int max)
    {
        int ran = UnityEngine.Random.Range(min, max);
        return ran;
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
                (transform.position, currentWaypoint, moveSpeed * Time.deltaTime);
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
