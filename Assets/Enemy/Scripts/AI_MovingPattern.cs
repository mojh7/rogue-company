using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 이동패턴 구현
public class AI_MovingPattern : MonoBehaviour
{
    // 1. 이동 : 1) 정지 2) 좌우 움직임 3)추적 4) 돌진 5) 시계방향 이동 6) 역추적

    #region variables
    [SerializeField]
    public GameObject player;

    // Asta
    Vector3[] path;
    public LayerMask unwalkableMask;
    int targetIndex;

    // 원형 이동
    private float radius = 3;
    private float runningTime = 0;
    private Vector3 newPos = new Vector3();

    // 돌진
    // 돌진의 Point는 Manager로 변경하자
    [HideInInspector]
    public GameObject point;

    [SerializeField]
    private float currentTime;              // 시간
    [SerializeField]
    public float moveDif;                          // 거리차
    private float speed = 0;                        // 이동속도를 받아올 변수
    private Vector3 startDir = Vector3.zero; // LR, UD 의 시작 점
    [HideInInspector]
    public bool isTurn = true;              // 몬스터 y축 회전 bool
    [HideInInspector]
    public bool isPoint = false;            // 대쉬 여부 확인

    public static AI_MovingPattern instance;    // 인스턴스


    //public float test_StopTr = 3f;
    //public bool isPath = false;                 // Astar bool
    #endregion

    #region getset
    public bool _rotate
    { get { return isTurn; } set { isTurn = value; } }
    public bool _point
    { get { return isPoint; } set { isPoint = value; }}

    #endregion

    private void Awake()
    {
        player = GameObject.Find("Player");
        if (instance == null)
        {
            instance = this;
        }
        // Init
        startDir = this.transform.position;
        isTurn = true;
        if (isPoint)
            point = transform.Find("point").gameObject;
    }

    void FixedUpdate()
    {
        // 거리차
        moveDif = Vector3.Distance(transform.position, player.transform.position);
        //Debug.Log("거리차 : " + moveDif);

        // 몬스터 회전.  
        if (isTurn)
        {
            RotationOjbect();
        }
    }

    #region moveFun

    public void RotationOjbect()
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

    public void Stop()
    {
        isTurn = false;
        speed = 0;
        Debug.Log("정지상태");
    }

    public void Track(float trackDis, float trackSpeed)
    {
        Debug.Log("??");
        speed = trackSpeed;
        if (moveDif > trackDis)
            PathRequestManager.RequestPath(new PathRequest(transform.position, player.transform.position, OnPathFound));
    }

    public void Untrack()
    {

    }

    public void Dash(float dashDis, float dashSpeed)
    {
        // 만일 장애물(Mask사용)이 없고, 플레이어의 거리차가 2이하이면 돌진.
        //int layerMask = (-1) - ((1 << unwalkableMask));
        //RaycastHit hit;
        //bool isMask = Physics.Raycast(transform.position,
        //    transform.TransformDirection(Vector3.right), out hit, Mathf.Infinity, layerMask);
        // 추가, 수정할 것 : mask

        speed = dashSpeed;
        bool isDash = moveDif < 6;              // 대시범위 안에 있을 때 true
        bool isAttack = moveDif > dashDis;     // 공격범위 밖에 있을 떄 true
        if (isDash && isAttack)
        {
            point.gameObject.SetActive(true);
            Vector3 dir = (player.transform.position - transform.position).normalized;
            this.transform.Translate(dir * speed * 2 * Time.deltaTime);
        }
        if (!isAttack)
        {
            point.gameObject.SetActive(false);
        }
    }

    int ran;
    int LRpos = 0;        // 왼쪽 오른쪽
    int UDpos = 0;        // 아래 위래
    public void MoveLR(float moveSpeed)
    {
        currentTime += Time.deltaTime;
        if (currentTime > 0.5f && currentTime < 1.5f)
            ran = RandomInt(0, 2);
        if (currentTime > 2f)
        {
            if (ran == 0)
            {
                if (LRpos == 0)
                {
                    //Vector3 dir = (new Vector3(transform.position.x + 2, transform.position.y, transform.position.z) - transform.position).normalized;
                    if (transform.position.x < startDir.x + 2)
                    {
                        transform.rotation = Quaternion.Euler(0, 0, 0);
                        this.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
                    }
                    else
                    {
                        currentTime = 0;
                        LRpos = 1;
                        return;
                    }
                    print("오른쪽으로 이동이욤");
                }
                else if (LRpos == 1)
                {
                    // Vector3 dir = (new Vector3(transform.position.x, transform.position.y, transform.position.z) - transform.position).normalized;
                    if (transform.position.x > startDir.x)
                    {
                        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
                        transform.rotation = Quaternion.Euler(0, 180, 0);
                    }
                    else
                    {
                        currentTime = 0;
                        LRpos = 0;
                        return;
                    }
                    print("왼쪽으로 이동이욤");
                }
            }
            else if (ran == 1)
            {
                if (UDpos == 0)
                {
                    //Vector3 dir = (new Vector3(transform.position.x, transform.position.y + 2, transform.position.z) - transform.position).normalized;
                    if (transform.position.y < startDir.y + 2)
                    {
                        this.transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
                    }
                    else
                    {
                        currentTime = 0;
                        UDpos = 1;
                        return;
                    }
                    print("위쪽으로 이동이욤");
                }
                else if (UDpos == 1)
                {
                    //Vector3 dir = (new Vector3(transform.position.x, transform.position.y, transform.position.z) - transform.position).normalized;
                    if (transform.position.y > startDir.y)
                    {
                        this.transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
                    }
                    else
                    {
                        currentTime = 0;
                        UDpos = 0;
                        return;
                    }
                    print("아래쪽으로 이동이욤");
                }
            }
            Debug.Log(ran);
        }
    }

    bool isCir = false;          // Circle이동하는 범위에 들어가면 true
    bool startPoint = false;     // Circle의 시작점에 위치하면 true
    bool isArc = false;                // Player와 Enemy 사이의 Angle에 해당하는 호의 한 점이 정해졌으면 true, false일 때 runningTime이 실행됨
    Vector3 v = Vector3.zero;           // Player와 거리점 사이의 벡터 차
    float degree = 0;                   // Player와 거리점 사이의 Angle
    Vector3 v2 = Vector3.zero;          // Player와 Enmey 사이의 벡터 차
    float degree2 = 0;                  // Player와 Enemy 사이의 Angle
    Vector3 arcPos = Vector3.zero;     // Enemy가 이동하게 될 호의 한 점

    public void MoveCir(float cirDis, float cirSpeed)
    {
        speed = cirSpeed;
        // Q) 플레이어 인식 범위는 어떻게??
        if (moveDif < cirDis + 2)
            isCir = true;

        // 몬스터가 일정 범위(Test - 5)에 들어가면 Enemy는 angle위치로 이동.
        if (isCir)
        {
            if (!startPoint)
            {
                StartCoroutine("TestCoroutine");
            }
            else
            {
                StopCoroutine("TestCoroutine");
                runningTime += Time.deltaTime * .5f;
                this.transform.position = newPos;
            }
        }
        if (!isArc)
        {
            runningTime += Time.deltaTime * 3f;
        }
        MonsterCircleMove(runningTime);
    }

    // Enemy가 반지름 시작되는 부분으로 움직이는 벡터
    IEnumerator TestCoroutine()
    {
        // 플레이어와 Enemy의 거리차가 3이하가 될 때 까지
        if (moveDif > 3)
        {
            // Enemy는 TestPos된 부분을 쫓아가기.
            this.transform.position = Vector3.MoveTowards(transform.position, arcPos, speed * Time.deltaTime);
        }
        else
        {
            // 플레이어와 Enemy의 거리차가 3이상이 되면 코루틴을 startPoint를 true로 반환하고
            // 코루틴은 종료가 된다.
            yield return startPoint = true;
        }
    }

    // Circle이 계속해서 돈다.
    public void MonsterCircleMove(float runningTime)
    {
        float x = player.transform.position.x + radius * Mathf.Cos(runningTime);
        float y = player.transform.position.y + radius * Mathf.Sin(runningTime);
        newPos = new Vector3(x, y, 0);

        v = newPos - player.transform.position;
        degree = (float)Math.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        v2 = transform.position - player.transform.position;
        degree2 = (float)Math.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;

        if ((int)degree + 1 >= (int)degree2 && (int)degree - 1 <= (int)degree2)
        {
            arcPos = newPos;
            isArc = true;
            Debug.Log("뿌꾸");
        }
    }

    public int RandomInt(int min, int max)
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
                (transform.position, currentWaypoint, speed * Time.deltaTime);
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
        if (!isCir || isCir)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(arcPos, new Vector3(0.5f, 0.5f));

            Gizmos.color = Color.black;
            Gizmos.DrawCube(newPos, new Vector3(0.5f, 0.5f));
        }
    }
    #endregion
}
