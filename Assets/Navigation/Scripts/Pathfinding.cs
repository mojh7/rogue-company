using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

public class Pathfinding : MonoBehaviour {

    //public Transform seeker, target;

    // PathRequestManager requestManager;
    GridPath grid;

    private void Awake()
    {
        // requestManager = GetComponent<PathRequestManager>();
        grid = GetComponent<GridPath>();
    }

    //void Update()
    //{
    //    if (Input.GetButtonDown("Jump"))
    //    {
    //        FindPath(seeker.position, target.position);
    //    }
    //}

    //public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    //{
    //    StartCoroutine(FindPath(startPos, targetPos));
    //}

    //IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    public void FindPath(PathRequest request, Action<PathResult> callback)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        //Node startNode = grid.NodeFromWorldPoint(startPos);
        //Node targetNode = grid.NodeFromWorldPoint(targetPos);

        Node startNode = grid.NodeFromWorldPoint(request.pathStart);
        Node targetNode = grid.NodeFromWorldPoint(request.pathEnd);
        startNode.parent = startNode;

        if (startNode.walkable && targetNode.walkable)
        {
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                //for (int i = 1; i < openSet.Count; i++)
                //{
                //    if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                //    {
                //        currentNode = openSet[i];
                //    }
                //}

                //openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    sw.Stop();
                    //print("Path Found : " + sw.ElapsedMilliseconds + " ms");
                    pathSuccess = true;
                    //RetracePath(startNode, targetNode);
                    break;
                }
                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbours = currentNode.gCost + GetDinstance(currentNode, neighbour);
                    if (newMovementCostToNeighbours < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbours;
                        neighbour.hCost = GetDinstance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                        else
                        {
                            openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }
        }
        //yield return null;
        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
            pathSuccess = waypoints.Length > 0;
        }
        //requestManager.FinishedProcessingPath(waypoints, pathSuccess);
        callback(new PathResult(waypoints, pathSuccess, request.callback));
    }

    Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector3[] waypoints = SimplifyPath(path);

        Array.Reverse(waypoints);
        return waypoints;

        //path.Reverse();
        //grid.path = path;
    }

    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX,
                path[i - 1].gridY - path[i].gridY);
            if (directionNew != directionOld)
            {
                waypoints.Add(path[i].worldPos);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }

    int GetDinstance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }
        return 14 * dstX + 10 * (dstY - dstX);
    }

}
