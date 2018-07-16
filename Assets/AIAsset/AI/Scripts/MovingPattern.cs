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
    #endregion
    #region components
    Rigidbody2D rb2d;
    #endregion
    #region variables
    bool isActive;
    float speed = 1;
    float baseSpeed;
    Vector2[] path;
    Vector2 zero = Vector2.zero;
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
    public void Go()
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
        rb2d.velocity = zero;
        isActive = false;
    }
    #endregion

    #region MovingInitialize
    /// <summary>
    /// 추적 클래스 생성.
    /// </summary>
    /// <param name="target">목표.</param>
    public void AStarTracker(Transform target)
    {
        aStarTracker = new AStarTracker(transform, ref target, Follwing);
    }
    /// <summary>
    /// 회전 추적 클래스 생성.
    /// </summary>
    /// <param name="target">목표.</param>
    /// <param name="radius">반지름 거리.</param>
    public void RoundingTracker(Transform target, float radius)
    {
        roundingTracker = new RoundingTracker(transform, ref target, Follwing, radius);
    }
    /// <summary>
    /// 돌진 추적 클래스 생성.
    /// </summary>
    /// <param name="target">목표.</param>
    public void RushTracker(Transform target)
    {
        rushTracker = new RushTracker(transform, ref target, Follwing);
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
    #endregion

    #region CallBack
    /// <summary>
    /// path를 추적하는 코루틴 함수 실행.
    /// </summary>
    /// <param name="path">추적 알고리즘에 의해 제공 된 매개변수.</param>
    void Follwing(Vector2[] path)
    {
        this.path = path;
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
            
            rb2d.velocity = dir.normalized * speed;
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
    protected Action<Vector2[]> callback;
    /// <summary>
    /// 타겟 설정.
    /// </summary>
    /// <param name="target">목표.</param>
    public void SetTarget(Transform target)
    {
        this.target = target;
    }
    /// <summary>
    /// 추적 알고리즘를 통해 path 업데이트.
    /// </summary>
    public abstract bool Update();
    /// <summary>
    /// 성공적으로 path를 찾았을 경우 AI컨트롤러의 FollowPath를 실행.
    /// </summary>
    /// <param name="newPath">알고리즘에 의해 반환 된 path.</param>
    /// <param name="pathSuccessful">알고리즘 성공 여부.</param>
    protected void OnPathFound(Vector2[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            callback(newPath);
        }
    }
}

class AStarTracker : Tracker
{

    public AStarTracker(Transform transform, ref Transform target, Action<Vector2[]> callback)
    {
        this.transform = transform;
        this.target = target;
        this.callback = callback;
    }

    public override bool Update()
    {
        if (transform == null || target == null)
            return false;
        AStar.PathRequestManager.RequestPath(new AStar.PathRequest(transform.position, target.position, OnPathFound));
        return true;
    }
}

class RoundingTracker : Tracker
{
    float radius;

    public RoundingTracker(Transform transform, ref Transform target, Action<Vector2[]> callback, float radius)
    {
        this.transform = transform;
        this.target = target;
        this.callback = callback;
        this.radius = radius;
    }

    public override bool Update()
    {
        if (transform == null || target == null)
            return false;
        AStar.PathRequestManager.RequestPath(new AStar.PathRequest(transform.position, target.position, OnPathFound), radius);
        return true;
    }

}

class RushTracker : Tracker
{
    public RushTracker(Transform transform, ref Transform target, Action<Vector2[]> callback)
    {
        this.transform = transform;
        this.target = target;
        this.callback = callback;
    }
    public override bool Update()
    {
        if (transform == null || target == null)
            return false;
        AStar.PathRequestManager.RequestPath(new AStar.PathRequest(transform.position, target.position, OnPathFound));
        return true;
    }
}