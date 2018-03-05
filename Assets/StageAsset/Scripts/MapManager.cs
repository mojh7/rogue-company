using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Map
{
    public class MapManager : MonoBehaviour
    {
        private static MapManager instance;
        Map map;
        public static MapManager Getinstance()
        {
            if(instance == null)
            {
                instance = GameObject.FindObjectOfType(typeof(MapManager)) as MapManager;
            }
            return instance;
        }

        private void Start()
        {
            map = new Map(10, 10, 3);

            //map.Generate();
        }

        private void Update()
        {
            if (Input.anyKeyDown)
            {
                GetComponent<Tilemap>().ClearAllTiles();
                map.Generate();
            }
        }

    }

    class Map
    {
        Rect mainRect;
        Queue<Rect> rects, halls, blocks, rooms;
        const int MinSplittableArea = 9;
        const float MaxHallRate = 0.2f;
        int TotalHallArea = 0;
        int size;

        public Map(int _width,int _height,int _size)
        {
            mainRect = new Rect(0, 0, _width, _height);
            size = _size;
            rects = new Queue<Rect>();
            halls = new Queue<Rect>();
            blocks = new Queue<Rect>();
            rooms = new Queue<Rect>();
        }

        void RefreshData()
        {
            rects.Clear();
            halls.Clear();
            blocks.Clear();
            TotalHallArea = 0;
        }

        public void Generate()
        {
            Random.InitState((int)System.DateTime.Now.Ticks);
            RefreshData();
            rects.Enqueue(mainRect);

            while(rects.Count > 0 && ((float)TotalHallArea / mainRect.area < MaxHallRate))
            {
                Rect rect;
                rect = rects.Dequeue();
                if (rect.area > MinSplittableArea)
                    SplitHall(rect);
                else blocks.Enqueue(rect);
            }
            while (rects.Count > 0)
            {
                blocks.Enqueue(rects.Dequeue());
            }
            while (blocks.Count > 0)
            {
                Rect block;
                block = blocks.Dequeue();
                if (block.area > 2)
                    SplitBlock(block);
                else
                    rooms.Enqueue(block);
            }
            FillTile();
        }

        void FillTile()
        {
            Tilemap tilemap = MapManager.Getinstance().GetComponent<Tilemap>();
            TileBase floor = TileManager.GetInstance().floorTile;
            TileBase wall = TileManager.GetInstance().wallTile;

            //while (blocks.Count > 0)
            //{
            //    Rect rect;
            //    rect = blocks.Dequeue();

            //    for (int i = rect.x; i < rect.x + rect.width; i++)
            //    {
            //        for (int j = rect.y; j < rect.y + rect.height; j++)
            //        {
            //            tilemap.SetTile(new Vector3Int(i, j, 0), wall);
            //        }
            //    }
            //}
            while (rooms.Count > 0)
            {
                Rect rect;
                rect = rooms.Dequeue();
                rect.DrawLine();
            }
        }

        void SplitHall(Rect _currentRect)
        {
            Rect hall = null;

            if (CoinFlip(80))
            {
                Rect rect_a = null, rect_b = null;
                RandomSplit(_currentRect, out rect_a, out hall, out rect_b);
                rects.Enqueue(rect_a);
                rects.Enqueue(rect_b);
            }
            else
            {
                Rect rect_a = null;
                RandomSplit(_currentRect, out rect_a, out hall);
                rects.Enqueue(rect_a);
            }

            halls.Enqueue(hall);
            TotalHallArea += hall.area;
        }

        void SplitBlock(Rect _currentBlock)
        {
            Rect block_a = null;
            Rect block_b = null;
            RandomSplit(_currentBlock, out block_a, out block_b);
            blocks.Enqueue(block_a);
            blocks.Enqueue(block_b);
        }

        void RandomSplit(Rect _currentRect, out Rect _rectA, out Rect _hall, out Rect _rectB)
        {
            bool flag = true;

            if (_currentRect.width > _currentRect.height)
            {
                flag = true;
            }
            else if (_currentRect.width < _currentRect.height)
            {
                flag = false;
            }
            else
            {
                if (CoinFlip(50))
                    flag = true;
                else
                    flag = false;
            }

            if(flag)
            {
                int x1 = _currentRect.x + _currentRect.width * Random.Range(3, 8) /10;
                int x2 = x1 + 1;
                _rectA = new Rect(_currentRect.x, _currentRect.y, x1 - _currentRect.x, _currentRect.height);
                _hall = new Rect(_rectA.x + _rectA.width, _currentRect.y, 1, _currentRect.height);
                _rectB = new Rect(_hall.x + _hall.width, _currentRect.y, _currentRect.width - _rectA.width - _hall.width, _currentRect.height);
            }
            else
            {
                int y1 = _currentRect.y + _currentRect.height * Random.Range(3, 8) / 10;
                int y2 = y1 + 1;
                _rectA = new Rect(_currentRect.x, _currentRect.y, _currentRect.width, y1 - _currentRect.y);
                _hall = new Rect(_currentRect.x, _rectA.y + _rectA.height, _currentRect.width, 1);
                _rectB = new Rect(_currentRect.x, _hall.y + _hall.height, _currentRect.width, _currentRect.height - _rectA.height - _hall.height);
            }
        }

        void RandomSplit(Rect _currentRect, out Rect _rectA, out Rect _hall)
        {
            bool flag = true;

            if (_currentRect.width > _currentRect.height)
            {
                flag = true;
            }
            else if (_currentRect.width < _currentRect.height)
            {
                flag = false;
            }
            else
            {
                if (CoinFlip(50))
                    flag = true;
                else
                    flag = false;
            }

            if (flag)
            {
                if(CoinFlip(50)) // left
                {
                    _hall = new Rect(_currentRect.x, _currentRect.y, 1, _currentRect.height);
                    _rectA = new Rect(_hall.x + 1, _currentRect.y, _currentRect.width - _hall.width, _currentRect.height);
                }
                else // right
                {
                    _hall = new Rect(_currentRect.x + _currentRect.width - 1, _currentRect.y, 1, _currentRect.height);
                    _rectA = new Rect(_currentRect.x, _currentRect.y, _currentRect.width - _hall.width, _currentRect.height);
                }
            }
            else
            {
                if (CoinFlip(50)) // down
                {
                    _hall = new Rect(_currentRect.x, _currentRect.y, _currentRect.width, 1);
                    _rectA = new Rect(_currentRect.x, _hall.y + 1, _currentRect.width, _currentRect.height - _hall.height);
                }
                else // up
                {
                    _hall = new Rect(_currentRect.x, _currentRect.y + _currentRect.height - 1, _currentRect.width, 1);
                    _rectA = new Rect(_currentRect.x, _currentRect.y, _currentRect.width, _currentRect.height - _hall.height);
                }
            }
        }

        void RandomBlockSplit(Rect _currentBlock, out Rect _blockA,out Rect _blockB)
        {
            bool flag = true;

            if (_currentBlock.width > _currentBlock.height)
            {
                flag = true;
            }
            else if (_currentBlock.width < _currentBlock.height)
            {
                flag = false;
            }
            else
            {
                if (CoinFlip(50))
                    flag = true;
                else
                    flag = false;
            }

            
            if(flag) // 가로
            {
                int width = _currentBlock.width * Random.Range(3, 8) / 10;

                _blockA = new Rect(_currentBlock.x, _currentBlock.y, width, _currentBlock.height);
                _blockB = new Rect(_currentBlock.x + width, _currentBlock.y, _currentBlock.width - width, _currentBlock.height);
            }
            else
            {
                int heigt = _currentBlock.width * Random.Range(3, 8) / 10;

                _blockA = new Rect(_currentBlock.x, _currentBlock.y, _currentBlock.width, heigt);
                _blockB = new Rect(_currentBlock.x, _currentBlock.y + heigt, _currentBlock.width, _currentBlock.height - heigt);
            }
        }

        bool CoinFlip(int percent)
        {
            return Random.Range(0, 100) < percent;
        }
    }

    class Rect
    {
        public readonly int x;
        public readonly int y;
        public readonly int width;
        public readonly int height;
        public readonly int area;
        public Rect(int _x,int _y,int _width,int _height)
        {
            x = _x;
            y = _y;
            width = _width;
            height = _height;
            area = width * height;
        }
        public void DrawLine()
        {
            Debug.DrawLine(new Vector2(x, y), new Vector2(x, y + height), Color.red, 1000);
            Debug.DrawLine(new Vector2(x, y + height), new Vector2(x + width, y + height), Color.red, 1000);
            Debug.DrawLine(new Vector2(x + width, y + height), new Vector2(x + width, y), Color.red, 1000);
            Debug.DrawLine(new Vector2(x + width, y), new Vector2(x, y), Color.red, 1000);
        }
    }


}


