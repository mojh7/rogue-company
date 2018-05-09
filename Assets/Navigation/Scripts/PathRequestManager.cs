using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class PathRequestManager : MonoBehaviour {

    Queue<PathResult> results = new Queue<PathResult>();

    //Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    //PathRequest currentPathRequest;

    static PathRequestManager instance;
    Pathfinding pathfinding;

    //bool isProcessingPath;

    private void Awake()
    {
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
    }

    private void Update()
    {
        if (results.Count > 0)
        {
            int itemsInQueue = results.Count;
            lock (results)
            {
                for (int i = 0; i < itemsInQueue; i++)
                {
                    PathResult result = results.Dequeue();
                    result.callback(result.path, result.success);
                }
            }
        }
    }

    public static void RequestPath(PathRequest request)
    {
        // 멀티쓰레드 x -> 파라미터
        //Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback
        // 본문
        //PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        //instance.pathRequestQueue.Enqueue(newRequest);
        //instance.TryProcessNext();
        ThreadStart threadStart = delegate
        {
            instance.pathfinding.FindPath(request, instance.FinishedProcessingPath);
        };
        threadStart.Invoke();
    }

    //private void TryProcessNext()
    //{
    //    if (!isProcessingPath && pathRequestQueue.Count > 0)
    //    {
    //        currentPathRequest = pathRequestQueue.Dequeue();
    //        isProcessingPath = true;
    //        pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
    //    }
    //}

    public void FinishedProcessingPath(PathResult result)
    {
        // 멀티쓰래드 x -> 파라미터
        //Vector3[] path, bool success
        //currentPathRequest.callback(path, success);
        //isProcessingPath = false;
        //TryProcessNext();
        
        //originalRequest.callback(path, success);
        //PathResult result = new PathResult(path, success, originalRequest.callback);
        lock (results)
        {
            results.Enqueue(result);
        }
    }
}

public struct PathResult
{
    public Vector3[] path;
    public bool success;
    public Action<Vector3[], bool> callback;

    public PathResult(Vector3[] path, bool success, Action<Vector3[], bool> callback)
    {
        this.path = path;
        this.success = success;
        this.callback = callback;
    }
}

public struct PathRequest
{
    public Vector3 pathStart;
    public Vector3 pathEnd;
    public Action<Vector3[], bool> callback;

    public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
    {
        pathStart = _start;
        pathEnd = _end;
        callback = _callback;
    }
}
