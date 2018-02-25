using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

namespace Maps
{
    public class Map : MonoBehaviour
    {
        private static Map instance;

        public GameObject m_startRoom;
        public GameObject m_endRoom;
        public GameObject[] m_monsterRoom;
        public GameObject[] m_itemRoom;
        public GameObject[] m_eventRoom;

        GameObject passageObj;
        Tilemap passageTileMap;

        Queue<Point> passagePositions;
        Room[,] roomArr;
        List<GameObject> roomList;

        int mapSize = 20;
        int width = 3;
        int height = 3;

        public static Map GetInstance()
        {
            if (!instance)
            {
                instance = GameObject.FindObjectOfType(typeof(Map)) as Map;
            }

            return instance;
        }

        private void Start()
        {
            InitMap();
            CreateRoom();
        }

        void InitMap()
        {
            roomArr = new Room[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    roomArr[i, j] = new Room(null, new Vector3(i * mapSize, j * mapSize), false);
                }
            }
            roomList = new List<GameObject>();
            passagePositions = new Queue<Point>();
            passageObj = new GameObject();
            passageObj.name = "Passage";
            passageObj.transform.parent = transform;
            passageObj.AddComponent<TilemapRenderer>();
            passageObj.AddComponent<TilemapCollider2D>().usedByComposite = true;
            passageObj.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            passageObj.AddComponent<CompositeCollider2D>();
            passageTileMap = passageObj.GetComponent<Tilemap>();
        }

        void CreateRoom()
        {
            ResetRoom();
            int roomNum = UnityEngine.Random.Range(7, 10);
            SetRoom(roomNum);
        }

        void ResetRoom()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    roomArr[i, j].isRoom = false;
                }
            }
        }

        void SetRoom(int _roomNum)
        {
            _roomNum -= 2;
            roomList.Add(m_startRoom);
            roomList.Add(m_endRoom);

            for (int i = 0; i < _roomNum; i++)
            {
                roomList.Add(m_monsterRoom[0]);
            }
            for (int i = roomList.Count; i < width * height; i++)
            {
                roomList.Add(null);
            }
            ShuffleList(roomList);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    GameObject roomObj = roomList[i * height + j];
                    if (roomObj == null)
                    {
                        continue;
                    }
                    roomArr[i, j].isRoom = true;
                    GameObject obj = LoadRoom(roomObj);
                    roomArr[i, j].SetObject(obj);
                    obj.transform.parent = transform;
                    obj.transform.position = roomArr[i, j].m_position;
                }
            }
            ConnectRoom();
        }

        void ConnectRoom()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (!roomArr[i, j].isRoom)
                        continue;
                    if (i <= width - 2)
                    {
                        if (roomArr[i + 1, j].isRoom)
                        {
                            MakePassage(roomArr[i, j], roomArr[i + 1, j], true);
                        }
                    }
                    if (j <= height - 2)
                    {
                        if (roomArr[i, j + 1].isRoom)
                        {
                            MakePassage(roomArr[i, j], roomArr[i, j + 1], false);
                        }
                    }
                }
            }
            MakePassageWall();
        }

        void MakePassage(Room A, Room B, bool isHorizon)
        {
            if (isHorizon)
            {
                Debug.DrawLine(A.m_door[1].m_position, B.m_door[0].m_position, Color.blue, 1000);
                WorkGird(A.m_door[1].m_position, B.m_door[0].m_position);
                A.m_doorList.Add(A.m_door[1].m_position);
                B.m_doorList.Add(B.m_door[0].m_position);
                A.SetDoor(A.m_door[1].m_position);
                B.SetDoor(B.m_door[0].m_position);
            }
            else
            {
                Debug.DrawLine(A.m_door[2].m_position, B.m_door[3].m_position, Color.blue, 1000);
                WorkGird(A.m_door[2].m_position, B.m_door[3].m_position);
                A.m_doorList.Add(A.m_door[2].m_position);
                B.m_doorList.Add(B.m_door[3].m_position);
                A.SetDoor(A.m_door[2].m_position);
                B.SetDoor(B.m_door[3].m_position);
            }
        }

        void WorkGird(Vector3Int p1, Vector3Int p2)
        {
            int dx = p1.x - p2.x, dy = p1.y - p2.y;
            int nx = Mathf.Abs(dx), ny = Mathf.Abs(dy);
            int sign_x = dx < 0 ? 1 : -1, sign_y = dy < 0 ? 1 : -1;
            Point p = new Point(p1.x, p1.y);
            TileBase floorTile = TileManager.GetInstance().floorTile;
            passageTileMap.SetTile(new Vector3Int(p.x, p.y, 0), floorTile);
            int ix, iy;
            for (ix = 0, iy = 0; ix < nx - 1 || iy < ny - 1;)
            {
                if ((0.5 + ix) / nx < (0.5 + iy) / ny)
                {
                    p.x += sign_x;
                    ix++;
                }
                else
                {
                    p.y += sign_y;
                    iy++;
                }
                passageTileMap.SetTile(new Vector3Int(p.x, p.y, 0), floorTile);
                passagePositions.Enqueue(new Point(p.x, p.y));
            }
            if ((0.5 + ix) / nx < (0.5 + iy) / ny)
            {
                p.x += sign_x;
                ix++;
            }
            else
            {
                p.y += sign_y;
                iy++;
            }
            passageTileMap.SetTile(new Vector3Int(p.x, p.y, 0), floorTile);
        }

        void MakePassageWall()
        {
            TileBase wallTile = TileManager.GetInstance().wallTile;

            while (passagePositions.Count > 0)
            {
                Point p = passagePositions.Dequeue();
                if (passageTileMap.GetTile(new Vector3Int(p.x - 1, p.y, 0)) == null)
                {
                    passageTileMap.SetTile(new Vector3Int(p.x - 1, p.y, 0), wallTile);
                }
                if (passageTileMap.GetTile(new Vector3Int(p.x + 1, p.y, 0)) == null)
                {
                    passageTileMap.SetTile(new Vector3Int(p.x + 1, p.y, 0), wallTile);
                }
                if (passageTileMap.GetTile(new Vector3Int(p.x, p.y + 1, 0)) == null)
                {
                    passageTileMap.SetTile(new Vector3Int(p.x, p.y + 1, 0), wallTile);
                }
                if (passageTileMap.GetTile(new Vector3Int(p.x, p.y - 1, 0)) == null)
                {
                    passageTileMap.SetTile(new Vector3Int(p.x, p.y - 1, 0), wallTile);
                }
            }
        }

        void ShuffleList<T>(List<T> list)
        {
            int n = list.Count;
            System.Random rnd = new System.Random();
            while (n > 1)
            {
                int k = (rnd.Next(0, n) % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        GameObject LoadRoom(GameObject _obj)
        {
            return UnityEngine.Object.Instantiate(_obj);
        }

    }

    class Room
    {
        GameObject m_roomObj;
        public Vector3 m_position;
        public bool isRoom;
        public Door[] m_door; // LEFT, RIGHT, UP, DOWN 
        public List<Vector3Int> m_doorList;
        public bool isLinked;

        public Room(GameObject _roomObj, Vector3 _position, bool _isRoom)
        {
            m_roomObj = _roomObj;
            m_position = _position;
            isRoom = _isRoom;
            m_door = new Door[4];
            m_doorList = new List<Vector3Int>();
            isLinked = false;
        }
        public GameObject GetObject()
        {
            return m_roomObj;
        }
        public void SetObject(GameObject _roomObj)
        {
            m_roomObj = _roomObj;
            SetDoorTile();
        }
        public void SetDoor(Vector3Int _position)
        {
            Tilemap tilemap = m_roomObj.GetComponent<Tilemap>();
            TileBase doorTile = TileManager.GetInstance().doorTile;
            tilemap.SetTile(new Vector3Int(_position.x - (int)m_position.x, _position.y - (int)m_position.y, 0), doorTile);
        }
        void SetDoorTile()
        {
            Tilemap tilemap = m_roomObj.GetComponent<Tilemap>();
            BoundsInt size = tilemap.cellBounds;
            TileBase wallTile = TileManager.GetInstance().wallTile;
            foreach (var position in tilemap.cellBounds.allPositionsWithin)
            {
                TileBase tile = tilemap.GetTile(position);

                if (null != tile)
                {
                    if (tile.name == "Door")
                    {
                        Vector3Int vector3 = new Vector3Int(position.x + (int)m_position.x, position.y + (int)m_position.y, 0);
                        if (tilemap.GetTile(new Vector3Int(position.x - 1, position.y, 0)) == null)
                        {
                            m_door[0] = new Door(tile,vector3,Direction.LEFT);
                        }
                        else if (tilemap.GetTile(new Vector3Int(position.x + 1, position.y, 0)) == null)
                        {
                            m_door[1] = new Door(tile, vector3, Direction.RIGHT);
                        }
                        else if (tilemap.GetTile(new Vector3Int(position.x, position.y + 1, 0)) == null)
                        {
                            m_door[2] = new Door(tile, vector3, Direction.TOP);
                        }
                        else
                        {
                            m_door[3] = new Door(tile, vector3, Direction.DOWN);
                        }
                        tilemap.SetTile(new Vector3Int(position.x, position.y, 0), wallTile);
                    }
                }
            }
        }
    }

    class Door
    {
        public TileBase m_tile;
        public Vector3Int m_position;
        public Direction m_direction;
        public int weight;

        public Door(TileBase _tile, Vector3Int _positon, Direction _direction)
        {
            m_tile = _tile;
            m_position = _positon;
            m_direction = _direction;
        }
    }

    class Point
    {
        public int x;
        public int y;
        public Point(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
    }

    enum Direction
    {
        LEFT, RIGHT, TOP, DOWN
    }

    class Heap<T> where T : IHeapItem<T>
    {

        T[] items;
        int currentItemCount;

        public Heap(int maxHeapSize)
        {
            items = new T[maxHeapSize];
        }

        public void Add(T item)
        {
            item.HeapIndex = currentItemCount;
            items[currentItemCount] = item;
            SortUp(item);
            currentItemCount++;
        }

        public T RemoveFirst()
        {
            T firstItem = items[0];
            currentItemCount--;
            items[0] = items[currentItemCount];
            items[0].HeapIndex = 0;
            SortDown(items[0]);
            return firstItem;
        }

        public void UpdateItem(T item)
        {
            SortUp(item);
        }

        public int Count
        {
            get
            {
                return currentItemCount;
            }
        }

        public bool Contains(T item)
        {
            return Equals(items[item.HeapIndex], item);
        }

        void SortDown(T item)
        {
            while (true)
            {
                int childIndexLeft = item.HeapIndex * 2 + 1;
                int childIndexRight = item.HeapIndex * 2 + 2;
                int swapIndex = 0;

                if (childIndexLeft < currentItemCount)
                {
                    swapIndex = childIndexLeft;

                    if (childIndexRight < currentItemCount)
                    {
                        if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                        {
                            swapIndex = childIndexRight;
                        }
                    }

                    if (item.CompareTo(items[swapIndex]) < 0)
                    {
                        Swap(item, items[swapIndex]);
                    }
                    else
                    {
                        return;
                    }

                }
                else
                {
                    return;
                }

            }
        }

        void SortUp(T item)
        {
            int parentIndex = (item.HeapIndex - 1) / 2;

            while (true)
            {
                T parentItem = items[parentIndex];
                if (item.CompareTo(parentItem) > 0)
                {
                    Swap(item, parentItem);
                }
                else
                {
                    break;
                }

                parentIndex = (item.HeapIndex - 1) / 2;
            }
        }

        void Swap(T itemA, T itemB)
        {
            items[itemA.HeapIndex] = itemB;
            items[itemB.HeapIndex] = itemA;
            int itemAIndex = itemA.HeapIndex;
            itemA.HeapIndex = itemB.HeapIndex;
            itemB.HeapIndex = itemAIndex;
        }
    }

    interface IHeapItem<T> : IComparable<T>
    {
        int HeapIndex
        {
            get;
            set;
        }
    }
}