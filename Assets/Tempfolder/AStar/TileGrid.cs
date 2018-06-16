using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AStar
{
    public class TileGrid : MonoBehaviourSingleton<TileGrid>
    {
        public bool displayGridGizmos;
        Node[,] grid;
        public LayerMask unwalkableMask;
        public float nodeRadius = 0.5f;
        float nodeDiameter;
        Vector2 gridWorldSize;
        int width, height;
        
        public int Width { get { return width; } }
        public int Height { get { return height; } }

        public int MaxSize
        {
            get
            {
                return width * height;
            }
        }

        public void Bake()
        {
            gridWorldSize = new Vector2(Map.MapManager.Instance.width * Map.MapManager.Instance.size + 1, Map.MapManager.Instance.height * Map.MapManager.Instance.size + 1);
            nodeDiameter = nodeRadius * 2;
            width = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
            height = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
            grid = new Node[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3 worldPoint = Vector3.right * (x * nodeDiameter + nodeRadius)
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

                    if (checkX >= 0 && checkX < width && checkY >= 0 && checkY < height)
                    {
                        neighbours.Add(grid[checkX, checkY]);
                    }
                }
            }
            return neighbours;
        }

        public Node GetWalkableNeighbours(Node node)
        {

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0 || (x * y != 0))
                        continue;

                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;

                    if (checkX >= 0 && checkX < width && checkY >= 0 && checkY < height)
                    {
                        if (grid[checkX, checkY].walkable)
                        {
                            return grid[checkX, checkY];
                        };
                    }
                }
            }
            return node;
        }
        public Node NodeFromWorldPoint(Vector3 worldPos)
        {
            float percentX = worldPos.x / gridWorldSize.x;
            float percentY = worldPos.y / gridWorldSize.y;

            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            int x = Mathf.RoundToInt((width - 1) * percentX);
            int y = Mathf.RoundToInt((height - 1) * percentY);

            return grid[x, y];
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, new Vector2(gridWorldSize.x, gridWorldSize.y));
            if (grid != null && displayGridGizmos)
            {
                foreach (Node n in grid)
                {
                    Gizmos.color = (n.walkable) ? Color.clear : Color.red;
                    Gizmos.DrawCube(n.worldPos, Vector3.one * (nodeDiameter - .1f));
                }
            }
        }
    }

    public class Node : IHeapItem<Node>
    {

        public bool walkable;
        public Vector3 worldPos;
        public int gridX;
        public int gridY;

        public int gCost;
        public int hCost;

        public Node parent;
        int heapIndex;

        public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
        {
            walkable = _walkable;
            worldPos = _worldPos;
            gridX = _gridX;
            gridY = _gridY;
        }

        public int fCost
        {
            get
            {
                return gCost + hCost;
            }
        }

        public int HeapIndex
        {
            get
            {
                return heapIndex;
            }
            set
            {
                heapIndex = value;
            }
        }
        public int CompareTo(Node nodeToCompare)
        {
            int compare = fCost.CompareTo(nodeToCompare.fCost);
            if (compare == 0)
            {
                compare = hCost.CompareTo(nodeToCompare.hCost);
            }
            return -compare;
        }
    }


}
