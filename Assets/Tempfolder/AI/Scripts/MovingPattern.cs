using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MovingPattern : MonoBehaviour
{
    Transform target;
    float speed = 1;
    float baseSpeed;
    Vector2[] path;
    AStarTracker aStarTracker;
    RoundingTracker roundingTracker;
    RushTracker rushTracker;

    private void Start()
    {
        baseSpeed = speed;
    }
    void OnEnable()
    {
        target = PlayerManager.Instance.GetPlayer().transform;
        this.GetComponent<BT.BehaviorTree>().Init();
    }

    #region Initialize
    public void AStarTracker()
    {
        aStarTracker = new AStarTracker(transform, ref target, Follwing);
    }
    public void RoundingTracker(float radius)
    {
        roundingTracker = new RoundingTracker(transform, ref target, Follwing, radius);
    }
    public void RushTracker()
    {
        rushTracker = new RushTracker(transform, ref target, Follwing);
    }
    #endregion

    #region Func
    public bool AStarTracking()
    {
        if (aStarTracker == null)
            return false;
        speed = baseSpeed;
        aStarTracker.Update();
        return true;
    }
    public bool RoundingTracking()
    {
        if (roundingTracker == null)
            return false;
        speed = baseSpeed;
        roundingTracker.Update();
        return true;
    }
    public bool RushTracking()
    {
        if (rushTracker == null)
            return false;
        speed = baseSpeed * 5;
        rushTracker.Update();
        return true;
    }
    #endregion

    #region CallBack
    void Follwing(Vector2[] path)
    {
        this.path = path;
        StopCoroutine("FollowPath");
        if (this.gameObject.activeSelf)
            StartCoroutine("FollowPath");
    }

    IEnumerator FollowPath()
    {
        int targetIndex = 0;
        Vector3 currentWaypoint = path[0];
        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }

            transform.position = Vector2.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
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

abstract class Tracker
{
    protected Transform target;
    protected Transform transform;
    protected Action<Vector2[]> callback;

    public abstract void Update();

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

    public override void Update()
    {
        AStar.PathRequestManager.RequestPath(new AStar.PathRequest(transform.position, target.position, OnPathFound));
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

    public override void Update()
    {
        AStar.PathRequestManager.RequestPath(new AStar.PathRequest(transform.position, target.position, OnPathFound), radius);
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
    public override void Update()
    {
        AStar.PathRequestManager.RequestPath(new AStar.PathRequest(transform.position, target.position, OnPathFound));
    }
}