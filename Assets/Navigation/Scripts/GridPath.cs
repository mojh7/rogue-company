using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPath : MonoBehaviour
{

    // public bool onlyDisplayPathGizmos;

    public bool displayGrid;
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;

    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        createGrid();
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    private void createGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];

        // Q1. 왜 원하는 축대로 나오지 않을까? -> 벡터 방향값 변경하기
        // Forward : Z Right : X, Up : Y forward -> up 변경
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;

        // Q2. 왜 Y축은 1/2 크기로 Grid가 형성될까?

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius)
                    + Vector3.up * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics2D.OverlapCircle(worldPoint, nodeRadius, unwalkableMask));
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

    // Player 존재
    public Node NodeFromWorldPoint(Vector3 worldPos)
    {
        float percentX = (worldPos.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPos.y + gridWorldSize.y / 2) / gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    public List<Node> path;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));

        //if (onlyDisplayPathGizmos)
        //{
        //    if (path != null)
        //    {
        //        foreach (Node n in path)
        //        {
        //            Gizmos.color = Color.black;
        //            Gizmos.DrawCube(n.worldPos, Vector3.one * (nodeDiameter - .1f));
        //        }
        //    }
        //}
        //else
        //{ //여기안에 이프 있었음 grid!=null }
        if (grid != null)
        {
            //Node playerNode = NodeFromWorldPoint(player.position);
            foreach (Node n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red;

                //if (n.walkable)
                //{
                //    Gizmos.color = Color.white;
                //}
                //else
                //{
                //    Gizmos.color = Color.red;
                //}

                //if (path != null)
                //{
                //    if (path.Contains(n))
                //    {
                //        Gizmos.color = Color.black;
                //    }
                //}

                Gizmos.DrawCube(n.worldPos, Vector3.one * (nodeDiameter - .1f));
            }
        }
    }
}
