using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MovingPattern : MonoBehaviour
{
 
    #region movingPattern
    AStarTracker aStarTracker;
    RoundingTracker roundingTracker;
    RushTracker rushTracker;
    RunawayTracker runawayTracker;
    StopTracker stopTracker;
    PositionTracker positionTracker;
    #endregion
    #region components
    Rigidbody2D rb2d;
    #endregion
    #region variables
    bool isActive;
    float speed = 1;
    float baseSpeed = 1;
    float doublingValue = 1;
    Vector2[] path;
    #endregion
    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    #region Func
    public void Init(float speed)
    {
        baseSpeed = speed;
        isActive = true;
    }
    public void Play()
    {
        isActive = true;
    }
    public void Stop()
    {
        if(!isActive)
        {
            return;
        }
        StopCoroutine("FollowPath");
        //TODO : zero하는 거 없애는 것 얘기 더 해보기. 넉백이 적용이 안되서 임시로 주석
        isActive = false;
    }
    #endregion

    #region MovingInitialize
    /// <summary>
    /// 추적 클래스 생성.
    /// </summary>
    /// <param name="target">목표.</param>
    public void AStarTracker(Transform target,float doublingValue)
    {
        aStarTracker = new AStarTracker(transform, ref target, Follwing, doublingValue);
    }
    /// <summary>
    /// 회전 추적 클래스 생성.
    /// </summary>
    /// <param name="target">목표.</param>
    /// <param name="radius">반지름 거리.</param>
    public void RoundingTracker(Transform target, float doublingValue, float radius)
    {
        roundingTracker = new RoundingTracker(transform, ref target, Follwing, doublingValue, radius);
    }
    /// <summary>
    /// 돌진 추적 클래스 생성.
    /// </summary>
    /// <param name="target">목표.</param>
    public void RushTracker(Transform target, float doublingValue)
    {
        rushTracker = new RushTracker(transform, ref target, Follwing, doublingValue);
    }
    /// <summary>
    /// 역추적 클래스 생성.
    /// </summary>
    /// <param name="target">목표.</param>
    public void RunawayTracker(Transform target, float doublingValue)
    {
        runawayTracker = new RunawayTracker(transform, ref target, Follwing, doublingValue);
    }
    public void StopTracker(Transform target)
    {
        stopTracker = new StopTracker(transform, ref target, Follwing);
    }
    public void PositionTracker(Transform target, float doublingValue)
    {
        positionTracker = new PositionTracker(transform, ref target, Follwing);
    }
    #endregion

    #region MovingFunc
    /// <summary>
    /// 추적 행동.
    /// </summary>
    public bool AStarTracking()
    {
        if (aStarTracker == null)
            return false;
        if (rushTracker != null)
        {
            rushTracker.isRun = false;
        }
        speed = baseSpeed;

        return aStarTracker.Update();
    }
    /// <summary>
    /// 회전 추적 행동.
    /// </summary>
    public bool RoundingTracking()
    {
        if (roundingTracker == null)
            return false;
        if (rushTracker != null)
        {
            rushTracker.isRun = false;
        }
        speed = baseSpeed;

        return roundingTracker.Update();
    }
    /// <summary>
    /// 돌진 추적 행동 (기본 속도가 5배로 증가).
    /// </summary>
    public bool RushTracking()
    {
        if (rushTracker == null)
            return false;
        speed = baseSpeed * 5;

        return rushTracker.Update();
    }
    /// <summary>
    /// 역추적 행동.
    /// </summary>
    public bool RunawayTracking()
    {
        if (runawayTracker == null)
            return false;
        if (rushTracker != null)
        {
            rushTracker.isRun = false;
        }
        speed = baseSpeed;

        return runawayTracker.Update();
    }
    /// <summary>
    /// 정지 행동
    /// </summary>
    /// <returns></returns>
    public bool StopTracking()
    {
        if (stopTracker == null)
            return false;
        if(rushTracker != null)
        {
            rushTracker.isRun = false;
        }
        speed = 0;

        return stopTracker.Update();
    }

    public bool PositionTracking(Vector2 position, ref bool arrived)
    {
        if (positionTracker == null)
            return false;
        if (rushTracker != null)
        {
            rushTracker.isRun = false;
        }
        speed = baseSpeed;
        if(null != path)
        {
            float dist = (position - path[0]).sqrMagnitude;
            if (dist < 1f)
            {
                arrived = true;
            }
            else
            {
                arrived = false;
            }
        }
        else
        {
            arrived = true;
        }
        return positionTracker.Update(position);
    }
    #endregion

    #region CallBack
    /// <summary>
    /// path를 추적하는 코루틴 함수 실행.
    /// </summary>
    /// <param name="path">추적 알고리즘에 의해 제공 된 매개변수.</param>
    void Follwing(Vector2[] path, float doublingValue)
    {
        this.path = path;
        this.doublingValue = doublingValue;
        StopCoroutine("FollowPath");
        if (this.gameObject.activeSelf && isActive)
            StartCoroutine("FollowPath");
    }
    /// <summary>
    /// path를 추적하는 코루틴.
    /// </summary>
    IEnumerator FollowPath()
    {
        int targetIndex = 0;
        Vector3 currentWaypoint = path[0];
        Vector3 position;
        while (true)
        {
            position = transform.position;
            if (position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }
            Vector2 dir = currentWaypoint - position;
            rb2d.velocity = dir.normalized * speed * doublingValue;
            yield return null;

        }
    }
    #endregion

    public void OnDrawGizmos()
    {
        int targetIndex = 0;
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one * 0.2f);

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
}
/// <summary>
/// 추상 추적 클래스.
/// </summary>
abstract class Tracker
{
    protected Transform target;
    protected Transform transform;
    protected Action<Vector2[], float> callback;
    protected float doublingValue = 1;
    /// <summary>
    /// 타겟 설정.
    /// </summary>
    /// <param name="target">목표.</param>
    public void SetTarget(Transform target)
    {
        this.target = target;
    }
    /// <summary>
    /// 성공적으로 path를 찾았을 경우 AI컨트롤러의 FollowPath를 실행.
    /// </summary>
    /// <param name="newPath">알고리즘에 의해 반환 된 path.</param>
    /// <param name="pathSuccessful">알고리즘 성공 여부.</param>
    protected void OnPathFound(Vector2[] newPath, bool pathSuccessful, float doublingValue)
    {
        if (pathSuccessful)
        {
            callback(newPath, doublingValue);
        }
    }
}

class AStarTracker : Tracker
{

    public AStarTracker(Transform transform, ref Transform target, Action<Vector2[], float> callback, float doublingValue)
    {
        this.transform = transform;
        this.target = target;
        this.callback = callback;
        this.doublingValue = doublingValue;
    }

    public bool Update()
    {
        if (transform == null || target == null)
            return false;
        AStar.PathRequestManager.RequestPath(new AStar.PathRequest(transform.position, target.position, OnPathFound), doublingValue);
        return true;
    }
}

class RoundingTracker : Tracker
{
    float radius;

    public RoundingTracker(Transform transform, ref Transform target, Action<Vector2[], float> callback, float doublingValue, float radius)
    {
        this.transform = transform;
        this.target = target;
        this.callback = callback;
        this.radius = radius;
        this.doublingValue = doublingValue;
    }

    public bool Update()
    {
        if (transform == null || target == null)
            return false;
        AStar.PathRequestManager.RequestPath(new AStar.PathRequest(transform.position, target.position, OnPathFound), doublingValue, radius);
        return true;
    }

}

class RushTracker : Tracker
{
    public bool isRun;
    public RushTracker(Transform transform, ref Transform target, Action<Vector2[], float> callback, float doublingValue)
    {
        this.transform = transform;
        this.target = target;
        this.callback = callback;
        this.doublingValue = doublingValue;
        isRun = false;
    }
    public bool Update()
    {
        if (transform == null || target == null)
            return false;
        if (!isRun)
        {
            isRun = true;
            callback(new Vector2[1] { transform.position + 10 * (target.position - transform.position) }, doublingValue);
        }

        return true;
    }
}

class RunawayTracker : Tracker
{
    Vector3 leftDown, rightTop, leftTop, rightDown;
    float minX, maxX;
    float minY, maxY;

    public RunawayTracker(Transform transform, ref Transform target, Action<Vector2[], float> callback, float doublingValue)
    {
        this.transform = transform;
        this.target = target;
        this.callback = callback;
        this.doublingValue = doublingValue;
        RoomManager.Instance.GetCurrentRoomBound(out leftDown, out rightTop);
        minX = leftDown.x + 0.5f;
        maxX = rightTop.x - 0.5f;
        minY = leftDown.y + 0.5f;
        maxY = rightTop.y - 0.5f;
        leftDown = new Vector3(minX, minY);
        leftTop = new Vector3(minX, maxY);
        rightDown = new Vector3(maxX, minY);
        rightTop = new Vector3(maxX, maxY);
    }
    public bool Update()
    {
        if (transform == null || target == null)
            return false;
        if (transform.position.x < minX || transform.position.x > maxX ||
            transform.position.y < minY || transform.position.y > maxY)
        {
            //TODO : 방법이 조금 바껴야함.
            if (transform.position.x < minX)
            {
                AStar.PathRequestManager.RequestPath(new AStar.PathRequest(transform.position, leftDown, OnPathFound), doublingValue);

            }
            else if (transform.position.x > maxX)
            {
                AStar.PathRequestManager.RequestPath(new AStar.PathRequest(transform.position, rightTop, OnPathFound), doublingValue);

            }
            else if (transform.position.y < minY)
            {
                AStar.PathRequestManager.RequestPath(new AStar.PathRequest(transform.position, rightDown, OnPathFound), doublingValue);

            }
            else if (transform.position.y > maxY)
            {
                AStar.PathRequestManager.RequestPath(new AStar.PathRequest(transform.position, leftTop, OnPathFound), doublingValue);

            }
        }
        else
        {
            AStar.PathRequestManager.RequestPath(new AStar.PathRequest(transform.position, 2 * transform.position - target.position, OnPathFound), doublingValue);
        }
        return true;
    }
}

class StopTracker : Tracker
{
    public StopTracker(Transform transform, ref Transform target,Action<Vector2[], float> callback)
    {
        this.transform = transform;
        this.target = target;
        this.callback = callback;
    }
    public bool Update()
    {
        if (transform == null || target == null)
            return false;
        callback(new Vector2[1] { transform.position }, 0);
        return true;
    }
}

class PositionTracker : Tracker
{
    public PositionTracker(Transform transform, ref Transform target, Action<Vector2[], float> callback)
    {
        this.transform = transform;
        this.target = target;
        this.callback = callback;
    }
    public bool Update(Vector2 position)
    {
        if (transform == null || target == null)
            return false;
        AStar.PathRequestManager.RequestPath(new AStar.PathRequest(transform.position, position, OnPathFound), doublingValue);
        return true;
    }
}