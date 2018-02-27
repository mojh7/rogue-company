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
        int width = 4;
        int height = 4;

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
        public int GetWidht()
        {
            return width;
        }
        public int GetHeight()
        {
            return height;
        }
        void InitMap()
        {
            roomArr = new Room[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    roomArr[i, j] = new Room(null, new Point(i, j), new Vector3(i * mapSize, j * mapSize), RoomType.NONE);
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
            int roomNum = UnityEngine.Random.Range(width*height-width, width * height);
            SetRoom(roomNum);
        }
        void ResetRoom()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    roomArr[i, j].isRoom = RoomType.NONE;
                }
            }
        }
        void SetRoom(int _roomNum)
        {
            _roomNum -= 2;
            roomList.Add(m_startRoom);
            roomList.Add(m_endRoom);
            Room startRoom = null;
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
                        roomArr[i, j].isRoom = RoomType.NONE;
                        roomArr[i, j].SetObject();
                        continue;
                    }
                    if (roomObj == m_startRoom)
                    {
                        roomArr[i, j].isRoom = RoomType.START;
                        startRoom = roomArr[i, j];
                    }
                    else if (roomObj == m_endRoom)
                    {
                        roomArr[i, j].isRoom = RoomType.END;
                    }
                    else
                    {
                        roomArr[i, j].isRoom = RoomType.OTHER;
                    }
                    GameObject obj = LoadRoom(roomObj);
                    roomArr[i, j].SetObject(obj);
                    obj.transform.parent = transform;
                    obj.transform.position = roomArr[i, j].mPosition;
                }
            }
            ConnectRoom();
            LinkAllRoom(startRoom);
        }
        void ConnectRoom()
        {
            Room room = null;
            List<Room> rooms = new List<Room>();
            int x = 0, y = 0;
            for (x = 0; x < width; x++)
            {
                for (y = 0; y < height; y++)
                {
                    if (roomArr[x, y].isRoom == RoomType.START)
                    {
                        room = roomArr[x, y];
                        break;
                    }
                }
                if (room != null)
                    break;
            }
            rooms.Add(room);
            while (true)
            {
                bool isConnected = false;
                Direction direction;
                for (int i = 0; i < room.mDoor.Count; i++)
                {
                    int j = 0, k = 0;
                    direction = room.mDoor[i].mDirection;
                    switch (direction)
                    {
                        case Direction.LEFT:
                            j = -1;
                            break;
                        case Direction.RIGHT:
                            j = 1;
                            break;
                        case Direction.TOP:
                            k = 1;
                            break;
                        case Direction.DOWN:
                            k = -1;
                            break;
                    }
                    if (roomArr[x + j,y + k].isLinked && UnityEngine.Random.Range(0, 10) <= 8)
                        continue;

                    if (roomArr[x + j, y + k].isRoom == RoomType.END) // 연결할 방이 엔딩 방일 경우
                    {
                        if (roomArr[x + j, y + k].isLinked)
                            continue;
                        else
                        {
                            if (room.isRoom == RoomType.START) // 현재 방이 첫번째 방일 경우
                            {
                                continue;
                            }
                            else // 현재 방이 첫번째 방이 아닐 경우 연결해야함
                            {
                                LinkRoom(room, roomArr[x + j, y + k], direction);
                            }
                        }

                    }
                    else // 연결할 방이 평범한 방일 경우
                    {
                        LinkRoom(room, roomArr[x + j, y + k], direction);
                        room = roomArr[x + j, y + k];
                        rooms.Add(room);
                    }
                    isConnected = true;
                    break;
                }

                if (!isConnected)
                {              
                    rooms.Remove(room);
                    if (room.mConfirmDoorList.Count == 1 && room.isRoom == RoomType.NONE)
                    {
                        room = BackLink(room);
                    }
                    else
                    {
                        ShuffleList(rooms);
                        while (rooms.Count >= 0)
                        {
                            if (rooms.Count == 0)
                            {
                                return;
                            }
                            if (rooms[0].mDoor.Count == 0)
                            {
                                rooms.RemoveAt(0);
                            }
                            else
                            {
                                room = rooms[0];
                                break;
                            }
                        }
                    }
                }
                x = room.mPoint.x;
                y = room.mPoint.y;
            } // 트리 모양으로 연걸.
        }
        void LinkRoom(Room A, Room B, Direction direction)
        {
            A.isLinked = true;
            B.isLinked = true;
            for(int i = 0; i < A.mDoor.Count; i++)
            {
                if(A.mDoor[i].mDirection == direction)
                {
                    A.mConfirmDoorList.Add(A.mDoor[i]);
                    A.mDoor.RemoveAt(i);
                    break;
                }
            }
            Direction reverseDir = Direction.DOWN;
            switch (direction)
            {
                case Direction.LEFT:
                    reverseDir = Direction.RIGHT;
                    break;
                case Direction.RIGHT:
                    reverseDir = Direction.LEFT;
                    break;
                case Direction.TOP:
                    reverseDir = Direction.DOWN;
                    break;
                case Direction.DOWN:
                    reverseDir = Direction.TOP;
                    break;
            }
            for (int i = 0; i < B.mDoor.Count; i++)
            {
                if (B.mDoor[i].mDirection == reverseDir)
                {
                    B.mConfirmDoorList.Add(B.mDoor[i]);
                    B.mDoor.RemoveAt(i);
                    break;
                }
            }
        } //방 연결
        Room BackLink(Room A)
        {
            Direction direction = A.mConfirmDoorList[0].mDirection; 

            Direction reverseDir = Direction.DOWN;
            Room B = null;

            switch (direction)
            {
                case Direction.LEFT:
                    reverseDir = Direction.RIGHT;
                    B = roomArr[A.mPoint.x - 1, A.mPoint.y];
                    break;
                case Direction.RIGHT:
                    reverseDir = Direction.LEFT;
                    B = roomArr[A.mPoint.x + 1, A.mPoint.y];
                    break;
                case Direction.TOP:
                    reverseDir = Direction.DOWN;
                    B = roomArr[A.mPoint.x, A.mPoint.y + 1];
                    break;
                case Direction.DOWN:
                    reverseDir = Direction.TOP;
                    B = roomArr[A.mPoint.x, A.mPoint.y - 1];
                    break;
            }

            for (int i = 0; i < B.mConfirmDoorList.Count; i++)
            {
                if (B.mConfirmDoorList[i].mDirection == reverseDir)
                {
                    B.mConfirmDoorList.RemoveAt(i);
                    break;
                }
            }

            return B;
        } // 방 뒤로가기.
        void LinkAllRoom(Room room)
        {
            room.visited = true;
            Direction direction;
            int j, k;
            for (int i = 0; i < room.mConfirmDoorList.Count; i++)
            {
                j = 0;
                k = 0;
                if (room.mConfirmDoorList[i].visited)
                    continue;
                direction = room.mConfirmDoorList[i].mDirection;
                room.mConfirmDoorList[i].visited = true;
                switch (direction)
                {
                    case Direction.LEFT:
                        j = -1;
                        break;
                    case Direction.RIGHT:
                        j = 1;
                        break;
                    case Direction.TOP:
                        k = 1;
                        break;
                    case Direction.DOWN:
                        k = -1;
                        break;
                }
                Debug.DrawLine(room.mPosition, roomArr[room.mPoint.x + j, room.mPoint.y + k].mPosition, Color.red, 10000);
                LinkAllRoom(roomArr[room.mPoint.x + j, room.mPoint.y + k]);
            }
        }

        void MakeCorridor(Room A, Room B)
        {

        }
        //void MakePassage(Room A, Room B, bool isHorizon)
        //{
        //    if (isHorizon)
        //    {
        //        Debug.DrawLine(A.mDoor[1].mPosition, B.mDoor[0].mPosition, Color.blue, 1000);
        //        WorkGird(A.mDoor[1].mPosition, B.mDoor[0].mPosition);
        //        A.m_doorList.Add(A.mDoor[1].mPosition);
        //        B.m_doorList.Add(B.mDoor[0].mPosition);
        //        A.SetDoor(A.mDoor[1].mPosition);
        //        B.SetDoor(B.mDoor[0].mPosition);
        //    }
        //    else
        //    {
        //        Debug.DrawLine(A.mDoor[2].mPosition, B.mDoor[3].mPosition, Color.blue, 1000);
        //        WorkGird(A.mDoor[2].mPosition, B.mDoor[3].mPosition);
        //        A.m_doorList.Add(A.mDoor[2].mPosition);
        //        B.m_doorList.Add(B.mDoor[3].mPosition);
        //        A.SetDoor(A.mDoor[2].mPosition);
        //        B.SetDoor(B.mDoor[3].mPosition);
        //    }
        //}
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

    enum RoomType
    {
        START, END, OTHER, NONE
    }

    enum Direction
    {
        LEFT, RIGHT, TOP, DOWN
    }

    class Room
    {
        GameObject mRoomObj;
        public Point mPoint;
        public Vector3 mPosition;
        public RoomType isRoom;
        public List<Door> mDoor; // LEFT, RIGHT, UP, DOWN 
        public List<Door> mConfirmDoorList;
        public bool isLinked;
        public bool visited;

        public Room(GameObject _roomObj, Point _point, Vector3 _position, RoomType _isRoom)
        {
            mRoomObj = _roomObj;
            mPoint = _point;
            mPosition = _position;
            isRoom = _isRoom;
            mDoor = new List<Door>();
            mConfirmDoorList = new List<Door>();
            isLinked = false;
            visited = false;
        }
        public GameObject GetObject()
        {
            return mRoomObj;
        }
        public void SetObject(GameObject _roomObj)
        {
            mRoomObj = _roomObj;
            SetDoorTile();
        }
        public void SetObject()
        {
            if (mPoint.x != 0) 
                mDoor.Add(new Door(null, Vector3Int.zero, Direction.LEFT));
            if(mPoint.x!=Map.GetInstance().GetWidht()-1)
                mDoor.Add(new Door(null, Vector3Int.zero, Direction.RIGHT));
            if (mPoint.y != Map.GetInstance().GetHeight() - 1)
                mDoor.Add(new Door(null, Vector3Int.zero, Direction.TOP));
            if (mPoint.y != 0)
                mDoor.Add(new Door(null, Vector3Int.zero, Direction.DOWN));
        }
        public void SetDoor(Vector3Int _position)
        {
            Tilemap tilemap = mRoomObj.GetComponent<Tilemap>();
            TileBase doorTile = TileManager.GetInstance().doorTile;
            tilemap.SetTile(new Vector3Int(_position.x - (int)mPosition.x, _position.y - (int)mPosition.y, 0), doorTile);
        }
        void SetDoorTile()
        {
            Tilemap tilemap = mRoomObj.GetComponent<Tilemap>();
            BoundsInt size = tilemap.cellBounds;
            TileBase wallTile = TileManager.GetInstance().wallTile;
            foreach (var position in tilemap.cellBounds.allPositionsWithin)
            {
                TileBase tile = tilemap.GetTile(position);

                if (null != tile)
                {
                    if (tile.name == "Door")
                    {
                        Vector3Int vector3 = new Vector3Int(position.x + (int)mPosition.x, position.y + (int)mPosition.y, 0);
                        if (tilemap.GetTile(new Vector3Int(position.x - 1, position.y, 0)) == null && mPoint.x != 0)
                        {
                            mDoor.Add(new Door(tile,vector3,Direction.LEFT));
                        }
                        else if (tilemap.GetTile(new Vector3Int(position.x + 1, position.y, 0)) == null && mPoint.x != Map.GetInstance().GetWidht() - 1)
                        {
                            mDoor.Add(new Door(tile, vector3, Direction.RIGHT));
                        }
                        else if (tilemap.GetTile(new Vector3Int(position.x, position.y + 1, 0)) == null && mPoint.y != Map.GetInstance().GetHeight() - 1)
                        {
                            mDoor.Add(new Door(tile, vector3, Direction.TOP));
                        }
                        else if(tilemap.GetTile(new Vector3Int(position.x, position.y - 1, 0)) == null && mPoint.y != 0)
                        {
                            mDoor.Add(new Door(tile, vector3, Direction.DOWN));
                        }
                        tilemap.SetTile(new Vector3Int(position.x, position.y, 0), wallTile);
                    }
                }
            }
        }
    }

    class Door
    {
        public TileBase mTile;
        public Vector3Int mPosition;
        public Direction mDirection;
        public bool visited;

        public Door(TileBase _tile, Vector3Int _positon, Direction _direction)
        {
            mTile = _tile;
            mPosition = _positon;
            mDirection = _direction;
            visited = false;
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
}