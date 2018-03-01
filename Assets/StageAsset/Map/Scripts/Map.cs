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

        public GameObject mStartRoom;
        public GameObject mEndRoom;
        public GameObject[] mMonsterRoom;
        public GameObject[] mItemRoom;
        public GameObject[] mEventRoom;
        public GameObject mDoorObjet;
        public Tilemap corridorTileMap;

        Queue<Point> corridorPoint;
        Room[,] roomArr;
        int[,] mapArr; // 0 = none , 1 = floor , 2 = wall, 3 = door
        List<GameObject> roomList;

        int mapSize = 20;
        int width = 4;
        int height = 4;

        private void Start()
        {
            InitMap();
            CreateRoom();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                ResetRoom();
                CreateRoom();
            }
        }
        #region GetSet
        public static Map GetInstance()
        {
            if (!instance)
            {
                instance = GameObject.FindObjectOfType(typeof(Map)) as Map;
            }

            return instance;
        }
        public int GetSize() { return mapSize; }
        public int GetWidth() { return width; }
        public int GetHeight() { return height; }
        public void SetMapArr(int i,int j,TileBase tile)
        {
            if(tile.name == "Floor")
            {
                mapArr[i, j] = 1;
            }
            else if(tile.name == "Wall")
            {
                mapArr[i, j] = 2;
            }
            else if (tile.name == "Door")
            {
                mapArr[i, j] = 3;
            }
            else
            {
                mapArr[i, j] = 0;
            }
        }
        #endregion
        void InitMap()
        {
            roomArr = new Room[width, height];
            mapArr = new int[width * mapSize, height * mapSize];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    roomArr[i, j] = new Room(null, new Point(i, j), new Vector3(i * mapSize, j * mapSize), RoomType.NONE);
                }
            }
            roomList = new List<GameObject>();
            corridorPoint = new Queue<Point>();
        } // 초반 데이터 초기화
        #region CreateRoom
        void CreateRoom()
        {
            ResetRoom();
            int roomNum = UnityEngine.Random.Range(width*height-width, width * height);
            SetRoom(roomNum);
            MakeCorridorWall();
            MakeDoor();
        } // 방을 만드는 함수. 너비 * 높이 - 너비 ~ 너비 * 높이 사이의 갯수의 방을 만듬
        void ResetRoom()
        {
            roomList.Clear();
            corridorPoint.Clear();
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    roomArr[i, j].ResetAll();
                }
            }
            for(int i = 0; i < width * mapSize; i++)
            {
                for(int j=0; j < height * mapSize; j++)
                {
                    mapArr[i, j] = 0; // 0 = none
                }
            }
            Transform[] childList = GetComponentsInChildren<Transform>();
            corridorTileMap.ClearAllTiles();
            for (int i = 2; i < childList.Length; i++)
            {
                Destroy(childList[i].gameObject);
            }
        } // 모든 방의 데이터 초기화
        void SetRoom(int _roomNum)
        {
            _roomNum -= 2;
            roomList.Add(mStartRoom);
            roomList.Add(mEndRoom);
            Room startRoom = null;
            for (int i = 0; i < _roomNum; i++)
            {
                roomList.Add(mMonsterRoom[0]);
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
                        roomArr[i, j].mRoomType = RoomType.NONE;
                        roomArr[i, j].SetObject();
                        continue;
                    }
                    if (roomObj == mStartRoom)
                    {
                        roomArr[i, j].mRoomType = RoomType.START;
                        startRoom = roomArr[i, j];
                    }
                    else if (roomObj == mEndRoom)
                    {
                        roomArr[i, j].mRoomType = RoomType.END;
                    }
                    else
                    {
                        roomArr[i, j].mRoomType = RoomType.OTHER;
                    }
                    GameObject obj = LoadRoom(roomObj);
                    roomArr[i, j].SetObject(obj);
                    obj.transform.parent = transform;
                    obj.transform.position = roomArr[i, j].mPosition;
                }
            }
            ConnectRoom();
            LinkAllRoom(startRoom);
        } // 방 생성 시 방 개수를 입력하여 방 배치.
        void ConnectRoom()
        {
            Room room = null;
            List<Room> rooms = new List<Room>();
            int x = 0, y = 0;
            for (x = 0; x < width; x++)
            {
                for (y = 0; y < height; y++)
                {
                    SortDoorList(x, y, roomArr[x, y].mDoor);
                    if (roomArr[x, y].mRoomType == RoomType.START)
                    {
                        room = roomArr[x, y];
                    }
                }
            }
            rooms.Add(room);
            if(room == null)
            {
                Debug.Log("A");
            }

            while (true)
            {
                x = room.mPoint.x;
                y = room.mPoint.y;
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
                    if (roomArr[x + j,y + k].isLinked) //이미 연결되었을 경우 20퍼센트 확률로 연결하여 사이클 생성
                    {
                        if(roomArr[x + j, y + k].mRoomType == RoomType.NONE || UnityEngine.Random.Range(0, 10) >= 2)
                            continue;
                    }
                    if (roomArr[x + j, y + k].mRoomType == RoomType.END) // 연결할 방이 엔딩 방일 경우
                    {
                        if (roomArr[x + j, y + k].isLinked)
                            continue;
                        else
                        {
                            if (room.mRoomType == RoomType.START) // 현재 방이 첫번째 방일 경우
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
                    rooms.Remove(room); // 연결이 안됬을 경우 더이상 연결할 방이 없다는 뜻이므로 연결 가능한 룸 리스트에서 삭제
                    if (room.mConfirmDoorList.Count == 1 && room.mRoomType == RoomType.NONE) // 만약 none 방이고 다른 방과 연결되어 있지 않다면 뒤로 돌아가 삭제
                    {
                        room = BackLink(room);// 뒤로 돌아가 삭제하고 연결된 방을 리턴함.
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
            } // 트리 모양으로 연걸.
        } // 방을 데이터 상으로 연결
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
        } // 두개의 방 연결.
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
        } // 쓸모없는 방 삭제를 위한 함수
        void LinkAllRoom(Room room)
        {
            room.visited = true;
            Direction direction;
            int j, k;
            for (int i = 0; i < room.mConfirmDoorList.Count; i++) // room에 연결된 Door 리스트를 확인하여 링크.
            {
                j = 0;
                k = 0;
                direction = room.mConfirmDoorList[i].mDirection;
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
                if (roomArr[room.mPoint.x + j, room.mPoint.y + k].visited)
                    continue;
                LinkAllRoom(roomArr[room.mPoint.x + j, room.mPoint.y + k]);
                MakeCorridor(room);
            }
        } // 모든 방 실제로 연결
        void MakeCorridor(Room A)
        {
            Direction direction,reverseDir = Direction.DOWN;
            int j, k;
            int x = A.mPoint.x;
            int y = A.mPoint.y;
            Vector3Int bPos = Vector3Int.zero;
            TileBase floorTile = TileManager.GetInstance().floorTile;

            for (int i = 0; i < A.mConfirmDoorList.Count; i++)
            {
                j = 0;k = 0;
                direction = A.mConfirmDoorList[i].mDirection;
                switch (direction)
                {
                    case Direction.LEFT:
                        j = -1;
                        reverseDir = Direction.RIGHT;
                        break;
                    case Direction.RIGHT:
                        j = 1;
                        reverseDir = Direction.LEFT;
                        break;
                    case Direction.TOP:
                        k = 1;
                        reverseDir = Direction.DOWN;
                        break;
                    case Direction.DOWN:
                        k = -1;
                        reverseDir = Direction.TOP;
                        break;
                }
                foreach(Door bDoor in roomArr[x + j, y + k].mConfirmDoorList)
                {
                    if (bDoor.mDirection == reverseDir)
                    {
                        bPos = bDoor.mPosition;
                        break;
                    }
                }
                Vector3Int midPos = new Vector3Int((bPos.x + A.mConfirmDoorList[i].mPosition.x)/2, (bPos.y + A.mConfirmDoorList[i].mPosition.y)/2, 0); // 중간 점
                if (Direction.LEFT == direction || Direction.RIGHT == direction)
                {
                    foreach (int index in Range(midPos.x, A.mConfirmDoorList[i].mPosition.x))
                    {
                        corridorPoint.Enqueue(new Point(index, A.mConfirmDoorList[i].mPosition.y));
                        corridorTileMap.SetTile(new Vector3Int(index, A.mConfirmDoorList[i].mPosition.y, 0), floorTile);
                        Map.GetInstance().SetMapArr(index, A.mConfirmDoorList[i].mPosition.y, floorTile);
                    }
                    foreach (int index in Range(bPos.y, A.mConfirmDoorList[i].mPosition.y))
                    {
                        corridorPoint.Enqueue(new Point(midPos.x, index));
                        corridorTileMap.SetTile(new Vector3Int(midPos.x, index, 0), floorTile);
                        Map.GetInstance().SetMapArr(midPos.x, index, floorTile);
                    }
                    foreach (int index in Range(bPos.x, midPos.x))
                    {
                        corridorPoint.Enqueue(new Point(index, bPos.y));
                        corridorTileMap.SetTile(new Vector3Int(index, bPos.y, 0), floorTile);
                        Map.GetInstance().SetMapArr(index, bPos.y, floorTile);
                    }
                }
                else
                {
                    foreach (int index in Range(midPos.y, A.mConfirmDoorList[i].mPosition.y))
                    {
                        corridorPoint.Enqueue(new Point(A.mConfirmDoorList[i].mPosition.x, index));
                        corridorTileMap.SetTile(new Vector3Int(A.mConfirmDoorList[i].mPosition.x, index, 0), floorTile);
                        Map.GetInstance().SetMapArr(A.mConfirmDoorList[i].mPosition.x, index, floorTile);
                    }
                    foreach (int index in Range(bPos.x, A.mConfirmDoorList[i].mPosition.x))
                    {
                        corridorPoint.Enqueue(new Point(index, midPos.y));
                        corridorTileMap.SetTile(new Vector3Int(index, midPos.y, 0), floorTile);
                        Map.GetInstance().SetMapArr(index, midPos.y, floorTile);
                    }
                    foreach (int index in Range(midPos.y, bPos.y))
                    {
                        corridorPoint.Enqueue(new Point(bPos.x, index));
                        corridorTileMap.SetTile(new Vector3Int(bPos.x, index, 0), floorTile);
                        Map.GetInstance().SetMapArr(bPos.x, index, floorTile);
                    }
                } 
            }
        } // 문, 복도 만들기
        void MakeCorridorWall()
        {
            TileBase wallTile = TileManager.GetInstance().wallTile;
            while (corridorPoint.Count>0)
            {
                Point point = corridorPoint.Dequeue();
                int x = point.x;
                int y = point.y;

                for(int i = -1; i <= 1; i++)
                {
                    for(int j = -1; j <= 1; j++)
                    {
                        if (x + i < 0 || x + i > mapSize * width || y + j < 0 || y + j > mapSize * height)
                            continue;
                        if (mapArr[x + i, y + j] == 0)
                        {
                            mapArr[x + i, y + j] = 1;
                            corridorTileMap.SetTile(new Vector3Int(x + i, y + j, 0), wallTile);
                        }
                    }
                }
            }
        } // 복도 벽 만들기
        void MakeDoor()
        {
            for(int i = 0; i < width; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    if (roomArr[i,j].mRoomType != RoomType.NONE)
                    {
                        for (int k = 0; k < roomArr[i, j].mConfirmDoorList.Count; k++)
                        {
                            GameObject obj = LoadRoom(mDoorObjet);
                            obj.transform.parent = roomArr[i, j].GetObject().transform;
                            roomArr[i, j].mConfirmDoorList[k].SetObject(obj);
                            roomArr[i, j].SetDoor(roomArr[i, j].mConfirmDoorList[k].mPosition);
                        }
                    }//문 생성
                }
            }
        } // 문 오브젝트 생성
        void SortDoorList(int x, int y, List<Door> door)
        {
            int w = 0, h = 0;
            for (int i = 0; i < door.Count - 1; i++)
            {
                for (int j = i + 1; j < door.Count; j++)
                {
                    w = 0; h = 0;
                    switch (door[i].mDirection)
                    {
                        case Direction.LEFT:
                            w = -1;
                            break;
                        case Direction.RIGHT:
                            w = 1;
                            break;
                        case Direction.TOP:
                            h = 1;
                            break;
                        case Direction.DOWN:
                            h = -1;
                            break;
                    }
                    if (roomArr[x + w, y + h].mRoomType == RoomType.NONE)
                    {
                        Door temp = door[i];
                        door[i] = door[j];
                        door[j] = temp;
                    }
                }
            }
        }
        #endregion
        #region Others
        GameObject LoadRoom(GameObject _obj)
        {
            return UnityEngine.Object.Instantiate(_obj);
        } // 오브젝트 복사 소환
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
        } //리스트 셔플
        IEnumerable<int> Range(int start, int stop)
        {
            if (start <= stop)
            {
                for (int i = start; i <= stop; i++)
                    yield return i;
            }
            else
            {
                for (int i = start; i >= stop; i--)
                    yield return i;
            }
        } // int range 사이의 값을 순회하여 반환해줌.
        #endregion
    }

    enum RoomType
    {
        START, END, OTHER, NONE
    }

    enum Direction
    {
        LEFT, RIGHT, TOP, DOWN
    }

    public class MapManager : MonoBehaviour
    {
        Map map;
    }

    class Room
    {
        GameObject mObj;
        public Point mPoint;
        public Vector3 mPosition;
        public RoomType mRoomType;
        public List<Door> mDoor; // LEFT, RIGHT, UP, DOWN 
        public List<Door> mConfirmDoorList;
        public bool isLinked;
        public bool visited;
        public bool isClear;


        public Room(GameObject _roomObj, Point _point, Vector3 _position, RoomType _isRoom)
        {
            mObj = _roomObj;
            mPoint = _point;
            mPosition = _position;
            mRoomType = _isRoom;
            mDoor = new List<Door>(4);
            mConfirmDoorList = new List<Door>(4);
            isLinked = false;
            visited = false;
            isClear = false;
        }
        public void ResetAll()
        {
            mRoomType = RoomType.NONE;
            mDoor.Clear();
            mConfirmDoorList.Clear();
            isLinked = false;
            visited = false;
            isClear = false;
        }
        public GameObject GetObject()
        {
            return mObj;
        }
        public void SetObject(GameObject _obj)
        {
            mObj = _obj;
            SetDoorTile();
        }
        public void SetObject()
        {
            int mapSize = Map.GetInstance().GetSize();
            Vector3Int pos = new Vector3Int((int)(mPosition.x + mapSize / 2), (int)(mPosition.y + mapSize / 2), 0);

            if (mPoint.x != 0) 
                mDoor.Add(new Door( pos, Direction.LEFT));
            if(mPoint.x!=Map.GetInstance().GetWidth()-1)
                mDoor.Add(new Door( pos, Direction.RIGHT));
            if (mPoint.y != Map.GetInstance().GetHeight() - 1)
                mDoor.Add(new Door( pos, Direction.TOP));
            if (mPoint.y != 0)
                mDoor.Add(new Door( pos, Direction.DOWN));
        }
        public void SetDoor(Vector3Int _position)
        {
            Tilemap tilemap = mObj.GetComponent<Tilemap>();
            tilemap.SetTile(new Vector3Int(_position.x - (int)mPosition.x, _position.y - (int)mPosition.y, 0), null);
        }
        void SetDoorTile()
        {
            Tilemap tilemap = mObj.GetComponent<Tilemap>();
            BoundsInt size = tilemap.cellBounds;
            TileBase wallTile = TileManager.GetInstance().wallTile;
            foreach (var position in tilemap.cellBounds.allPositionsWithin)
            {
                TileBase tile = tilemap.GetTile(position);
                if (null != tile)
                {
                    Map.GetInstance().SetMapArr(position.x + (int)mPosition.x, position.y + (int)mPosition.y, tile);

                    if (tile.name == "Door")
                    {
                        Vector3Int vector3 = new Vector3Int(position.x + (int)mPosition.x, position.y + (int)mPosition.y, 0);
                        if (tilemap.GetTile(new Vector3Int(position.x - 1, position.y, 0)) == null && mPoint.x != 0)
                        {
                            mDoor.Add(new Door(vector3,Direction.LEFT));
                        }
                        else if (tilemap.GetTile(new Vector3Int(position.x + 1, position.y, 0)) == null && mPoint.x != Map.GetInstance().GetWidth() - 1)
                        {
                            mDoor.Add(new Door(vector3, Direction.RIGHT));
                        }
                        else if (tilemap.GetTile(new Vector3Int(position.x, position.y + 1, 0)) == null && mPoint.y != Map.GetInstance().GetHeight() - 1)
                        {
                            mDoor.Add(new Door(vector3, Direction.TOP));
                        }
                        else if(tilemap.GetTile(new Vector3Int(position.x, position.y - 1, 0)) == null && mPoint.y != 0)
                        {
                            mDoor.Add(new Door(vector3, Direction.DOWN));
                        }
                        tilemap.SetTile(new Vector3Int(position.x, position.y, 0), wallTile);
                    }
                }
            }
        }
    }

    class Door
    {
        public Vector3Int mPosition;
        public Direction mDirection;
        GameObject mObj;
        bool isOpen;

        public Door(Vector3Int _positon, Direction _direction)
        {
            mPosition = _positon;
            mDirection = _direction;
            isOpen = false;
        }

        public void SetObject(GameObject _obj)
        {
            mObj = _obj;
            mObj.transform.position = new Vector3(mPosition.x+0.5f,mPosition.y+0.5f,0);
        }

        public void ToggleDoor()
        {
            isOpen = !isOpen;
            mObj.GetComponent<BoxCollider2D>().enabled = isOpen;
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