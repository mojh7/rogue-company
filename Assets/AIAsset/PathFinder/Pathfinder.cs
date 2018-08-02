using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AStar
{
    public class Pathfinder : MonoBehaviourSingleton<Pathfinder>
    {
        TileGrid grid;
        Vector2[] waypoints;
        Heap<Node> openSet;
        HashSet<Node> closedSet;
        Vector2 directionOld;
        Vector2 zero;
        List<Vector2> waypointList;
        List<Node> pathList;

        private void Awake()
        {
            grid = GetComponent<TileGrid>();
        }

        public void Bake()
        {
            openSet = new Heap<Node>(grid.MaxSize);
            closedSet = new HashSet<Node>();
            zero = Vector2.zero;
            waypointList = new List<Vector2>();
            pathList = new List<Node>();
        }
        /// <summary>
        /// 기본 추적 알고리즘에 의해 path를 찾는 함수.
        /// </summary>
        public void FindPath(PathRequest request, Action<PathResult> callback)
        {

            bool pathSuccess = false;

            Node startNode = grid.NodeFromWorldPoint(request.pathStart);
            Node targetNode = grid.NodeFromWorldPoint(request.pathEnd);

            startNode.parent = startNode;

            if (!targetNode.walkable)
            {
                targetNode = TileGrid.Instance.GetWalkableNeighbours(targetNode);
            }
            if (startNode.walkable && targetNode.walkable)
            {
                openSet.RemoveAll();
                closedSet.Clear();
                openSet.Add(startNode);

                while (openSet.Count > 0)
                {
                    Node currentNode = openSet.RemoveFirst();
                    closedSet.Add(currentNode);

                    if (currentNode == targetNode)
                    {
                        pathSuccess = true;
                        break;
                    }

                    foreach (Node neighbour in grid.GetNeighbours(currentNode))
                    {
                        if (!neighbour.walkable || closedSet.Contains(neighbour))
                        {
                            continue;
                        }

                        int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                        if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                        {
                            neighbour.gCost = newMovementCostToNeighbour;
                            neighbour.hCost = GetDistance(neighbour, targetNode);
                            neighbour.parent = currentNode;

                            if (!openSet.Contains(neighbour))
                                openSet.Add(neighbour);
                            else
                                openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }
            if (pathSuccess)
            {
                waypoints = RetracePath(startNode, targetNode);
                pathSuccess = waypoints.Length > 0;
            }
            callback(new PathResult(waypoints, pathSuccess, request.callback));
        }
        /// <summary>
        /// 회전 추적 알고리즘에 의해 path를 찾는 함수.
        /// distance * Vector2(Cos(Θ) , Sin(Θ))
        /// 를 통해 계산된 Vector2와 가까운 Node를 List에 담아 path로 반환.
        /// </summary>
        public void FindPath(PathRequest request, Action<PathResult> callback, float raduis)
        {

            Vector2[] waypoints = new Vector2[0];
            bool pathSuccess = false;

            Node startNode = grid.NodeFromWorldPoint(request.pathStart);
            Node targetNode = grid.NodeFromWorldPoint(request.pathEnd);

            Vector2 anglePos = request.pathStart - request.pathEnd;
            float distance = raduis - 1;
            float rad = (float)Mathf.Atan2(anglePos.y, anglePos.x);
            startNode.parent = startNode;
            float tenRad = 10 * Mathf.Deg2Rad;
            float maxRad = 360 * Mathf.Deg2Rad;

            float term = 0;

            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                term += tenRad;
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (term > maxRad)
                {
                    targetNode = currentNode;
                    pathSuccess = true;
                    break;
                }

                Vector2 pos = targetNode.worldPos + distance * new Vector2(Mathf.Cos(rad + term), Mathf.Sin(rad + term));
                Node neighbour = grid.NodeFromWorldPoint(pos);

                if (neighbour != currentNode && !closedSet.Contains(neighbour))
                {
                    neighbour.parent = currentNode;
                }
                openSet.Add(neighbour);
            }
            
            if (pathSuccess)
            {
                waypoints = RetracePath(startNode, targetNode);
                pathSuccess = waypoints.Length > 0;
            }

            callback(new PathResult(waypoints, pathSuccess, request.callback));

        }

        Vector2[] RetracePath(Node startNode, Node endNode)
        {
            Node currentNode = endNode;
            pathList.Clear();

            while (currentNode != startNode)
            {
                pathList.Add(currentNode);
                currentNode = currentNode.parent;
            }
            Vector2[] waypoints = SimplifyPath(pathList);
            Array.Reverse(waypoints);
            return waypoints;

        }
 
        Vector2[] SimplifyPath(List<Node> path)
        {
            waypointList.Clear();
            directionOld = zero;

            int gap = 1;
            for (int i = gap; i < path.Count; i++)
            {
                Vector2 directionNew = new Vector2(path[i - gap].gridX - path[i].gridX, path[i - gap].gridY - path[i].gridY);
                if (directionNew != directionOld)
                {
                    waypointList.Add(path[i].worldPos);
                }
                directionOld = directionNew;
            }
            return waypointList.ToArray();
        }

        int GetDistance(Node nodeA, Node nodeB)
        {
            int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
            int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }

    }
}