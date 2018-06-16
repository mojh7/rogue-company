using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPattern : MonoBehaviour
{

    public Transform target;
    float speed = 1;
    Vector2[] path;
    int targetIndex;
    void Start()
    {
        target = PlayerManager.Instance.GetPlayer().transform;
    }

    private void Update()
    {
        AStar.PathRequestManager.RequestPath(new AStar.PathRequest(transform.position, target.position, OnPathFound));
    }

    public void OnPathFound(Vector2[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            Tracking();
        }
    }

    void Tracking()
    {
        StopCoroutine("FollowPath");
        if(this.gameObject.activeSelf)
            StartCoroutine("FollowPath");
    }

    IEnumerator FollowPath()
    {
        targetIndex = 0;
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

    public void OnDrawGizmos()
    {
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
