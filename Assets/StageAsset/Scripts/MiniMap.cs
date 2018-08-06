using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Threading;


public class MiniMap : MonoBehaviourSingleton<MiniMap>
{
    enum Direction { LEFT, RIGHT, TOP, DOWN }

    [SerializeField] private Sprite unknownIcon, monsterIcon, bossIcon, eventIcon, storeIcon;
    [SerializeField] private GameObject playerIcon;
    [SerializeField] private Transform mask;
    [SerializeField] private Text floorT;
    [SerializeField] private GameObject[] title;

    const int minimapBaseWidth = 400, minimapBaseHeight = 400;
    int minmapSizeWidth, minmapSizeHeight;
    List<Map.Rect> roomList;
    Texture2D texture;
    new RawImage renderer;
    float width, height;
    const int maskSize = 100;
    int size;
    float mapSizeWidth, mapSizeHeight; // 실제 맵 사이즈
    Vector2 playerPositon;
    Vector2 oldPos;
    bool isToggle = false;
    Color hallColor = new Color((float)160 / 255, (float)174 / 255, (float)186 / 255);
    // 0806 Test
    bool tanslateMapX = false;
    Vector2 mapV = Vector2.zero;
    Vector2 iconV = Vector2.zero;

    Direction Check(Map.Rect _rectA, Map.Rect _rectB)
    {
        if ((Mathf.Abs(_rectA.midX - _rectB.midX) == (float)(_rectA.width + _rectB.width) / 2) && (Mathf.Abs(_rectA.midY - _rectB.midY) < (float)(_rectA.height + _rectB.height) / 2))
        {
            if (_rectA.midX > _rectB.midX)
            {
                return Direction.LEFT;
            }
            return Direction.RIGHT;
        }
        else if ((Mathf.Abs(_rectA.midX - _rectB.midX) < (float)(_rectA.width + _rectB.width) / 2) && (Mathf.Abs(_rectA.midY - _rectB.midY) == (float)(_rectA.height + _rectB.height) / 2))
        {
            if (_rectA.midY > _rectB.midY)
            {
                return Direction.DOWN;
            }
            return Direction.TOP;
        }

        return Direction.RIGHT;
    }

    void titleMap(int activeTitle, int unActiveTitle)
    {
        title[activeTitle].SetActive(true);
        title[unActiveTitle].SetActive(false);
    }

    void EnableMask()
    {
        mask.GetComponent<Mask>().enabled = !mask.GetComponent<Mask>().enabled;
    }

    public void SetFloorText()
    {
        floorT.text = (5 + GameDataManager.Instance.GetFloor()).ToString() + "F";
    }

    public void ClearRoom(Map.Rect _room)
    {
        _room.isDrawed = true;
        DrawCall(_room, DrawRoom);
    }

    public void DrawMinimap()
    {
        SetFloorText();
        renderer = GetComponent<RawImage>();
        roomList = RoomManager.Instance.GetRoomList(); //리스트 받아오기
        size = 10; // 미니맵 
        minmapSizeWidth = Map.MapManager.Instance.width * size; // 미니맵 픽셀 사이즈
        minmapSizeHeight = Map.MapManager.Instance.height * size;

        mapSizeWidth = Map.MapManager.Instance.size * Map.MapManager.Instance.width; // 실제 맵 크기
        mapSizeHeight = Map.MapManager.Instance.size * Map.MapManager.Instance.height;

        if (Map.MapManager.Instance.width * size > Map.MapManager.Instance.height * size)
            GetComponent<RectTransform>().sizeDelta = new Vector2(minimapBaseWidth * (float)mapSizeWidth / mapSizeHeight, minimapBaseHeight);
        else
            GetComponent<RectTransform>().sizeDelta = new Vector2(minimapBaseWidth, minimapBaseHeight * (float)mapSizeHeight / mapSizeWidth);
        oldPos = new Vector2(mask.localPosition.x, mask.localPosition.y);

        width = GetComponent<RectTransform>().sizeDelta.x;
        height = GetComponent<RectTransform>().sizeDelta.y;

        texture = new Texture2D(minmapSizeWidth + 1, minmapSizeHeight + 1);
        texture.filterMode = FilterMode.Point;
        renderer.texture = texture;

        for (int i = 0; i <= minmapSizeWidth; i++)
        {
            for (int j = 0; j <= minmapSizeHeight; j++)
            {
                texture.SetPixel(i, j, Color.white);
            }
        }

        for (int i = 0; i < roomList.Count; i++)
        {
            if (!roomList[i].isRoom)
                DrawCall(roomList[i], DrawHall);
        }

        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].isRoom)
                DrawCall(roomList[i], DrawRoomOutline);
        }

        for (int i = 0; i < roomList.Count; i++)
        {
            if (!roomList[i].isRoom)
                DrawCall(roomList[i], DrawDoor);
        }

        for (int i = 0; i <= minmapSizeWidth; i++)
        {
            for (int j = 0; j <= minmapSizeHeight; j++)
            {
                if (i == 0 || i == minmapSizeWidth ||
                 j == 0 || j == minmapSizeHeight)
                {
                    texture.SetPixel(i, j, Color.black);
                }
            }
        }

        DrawAllRoom();
        texture.Apply();
    } // 미니맵 그리는 함수

    public void HideMiniMap()
    {
        this.gameObject.SetActive(!this.gameObject.activeSelf);
    }

    public void ToggleMinimap()
    {
        EnableMask();
        if (isToggle)
        {
            playerIcon.transform.localPosition = new Vector2(-maskSize, -maskSize);
            mask.localPosition = oldPos;
            GetComponent<RawImage>().color = new Color(1, 1, 1, 1);
            titleMap(0, 1);
        }
        else
        {
            transform.localPosition = new Vector2(-maskSize, -maskSize);
            mask.localPosition = new Vector2(maskSize, maskSize);
            GetComponent<RawImage>().color = new Color(1, 1, 1, 0.7f);
            titleMap(1, 0);
        }
        isToggle = !isToggle;
    }

    void DrawCall(Map.Rect _room,System.Action<Map.Rect> action)
    {
        ThreadStart threadStart = delegate
        {
            action(_room);
        };
        threadStart.Invoke();
    }
    
    void DrawIcon(Map.Rect _rect)
    {
        if (!_rect.isRoom)
            return;
        Sprite sprite;
        switch (_rect.eRoomType)
        {
            default:
            case RoomType.MONSTER:
                sprite = monsterIcon;
                break;
            case RoomType.BOSS:
                sprite = bossIcon;
                break;
            case RoomType.EVENT:
                sprite = eventIcon;
                break;
            case RoomType.STORE:
                sprite = storeIcon;
                break;
        }
        //if (!_rect.isClear)
        //    sprite = unknownIcon;
        int x = _rect.x * size + _rect.width * size / 2;
        int y = _rect.y * size + _rect.height * size / 2;
        Rect textureRect = sprite.textureRect;
        float width = textureRect.width;
        float height = textureRect.height;
        x -= (int)width / 2;
        y -= (int)height / 2;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Color color = sprite.texture.GetPixel((int)textureRect.x + i, (int)textureRect.y + j);
                if (color == Color.clear)
                {
                    texture.SetPixel(x + i, y + j, Color.white);
                }
                else
                {
                    texture.SetPixel(x + i, y + j, color);
                }
            }
        }
    } // 방 타입에 따른 미니맵 아이콘 표시

    void DrawAllRoom()
    {
        for(int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].isRoom)
                DrawCall(roomList[i], DrawRoom);
        }
    }

    void DrawRoom(Map.Rect _room)
    {
        int minX = _room.x * size;
        int maxX = (_room.x + _room.width) * size - 1;
        int minY = _room.y * size;
        int maxY = (_room.y + _room.height) * size - 1;

        float mapMidX = _room.midX * Map.MapManager.Instance.size;
        float mapMidY = _room.midY * Map.MapManager.Instance.size;

        float width = _room.width * _room.size;
        float height = _room.height * _room.size;
        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                if (x == minX || x == maxX ||
                    y == minY || y == maxY)
                {
                    if (y == maxY)
                    {
                        texture.SetPixel(x, y + 1, Color.black);
                        if (x == maxX)
                        {
                            texture.SetPixel(x, y, Color.white);
                            texture.SetPixel(x + 1, y, Color.black);
                            texture.SetPixel(x + 1, y + 1, Color.black);
                        }
                        else if (x == minX)
                        {
                            texture.SetPixel(x, y, Color.black);
                        }
                    }
                    else if (x == maxX)
                    {
                        texture.SetPixel(x, y, Color.white);
                        texture.SetPixel(x + 1, y, Color.black);
                        if (y == minY)
                        {
                            texture.SetPixel(x, y, Color.black);
                        }
                    }
                    else
                    {
                        texture.SetPixel(x, y, Color.black);
                    }
                }

                DrawCall(_room,DrawIcon);
            }
        }
        for (int i = 0; i < _room.doorObjects.Count; i++)
        {
            if (!_room.isRoom)
                break;
            bool horizon = _room.doorObjects[i].GetComponent<Door>().GetHorizon();

            float gap;

            if (!horizon) // 세로
            {
                gap = mapMidY - _room.doorObjects[i].transform.position.y;

                int pos = (minX + maxX) / 2;

                int doorPos = (int)Mathf.Floor(_room.doorObjects[i].transform.position.x);
                float x = doorPos / mapSizeWidth;
                pos = (int)(x * minmapSizeWidth);
                if (gap < 0)
                {
                    //top
                    texture.SetPixel(pos - 1, maxY + 1, Color.red);
                    texture.SetPixel(pos, maxY + 1, Color.red);
                    texture.SetPixel(pos + 1, maxY + 1, Color.red);
                }
                else
                {
                    //bottom
                    texture.SetPixel(pos - 1, minY, Color.red);
                    texture.SetPixel(pos, minY, Color.red);
                    texture.SetPixel(pos + 1, minY, Color.red);
                }
            }
            else
            {
                gap = mapMidX - _room.doorObjects[i].transform.position.x;

                int pos = (minY + maxY) / 2;

                int doorPos = (int)Mathf.Floor(_room.doorObjects[i].transform.position.y);
                float y = doorPos / mapSizeHeight;
                pos = (int)(y * minmapSizeHeight);

                if (gap < 0)
                {
                    //right
                    texture.SetPixel(maxX + 1, pos - 1, Color.red);
                    texture.SetPixel(maxX + 1, pos, Color.red);
                    texture.SetPixel(maxX + 1, pos + 1, Color.red);
                }
                else
                {
                    //left
                    texture.SetPixel(minX, pos - 1, Color.red);
                    texture.SetPixel(minX, pos, Color.red);
                    texture.SetPixel(minX, pos + 1, Color.red);
                }
            }

        }
        texture.Apply();
    }

    void DrawHall(Map.Rect _room)
    {
        int minX = _room.x * size;
        int maxX = (_room.x + _room.width) * size - 1;
        int minY = _room.y * size;
        int maxY = (_room.y + _room.height) * size - 1;

        float mapMidX = _room.midX * Map.MapManager.Instance.size;
        float mapMidY = _room.midY * Map.MapManager.Instance.size;

        float width = _room.width * _room.size;
        float height = _room.height * _room.size;
        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                texture.SetPixel(x, y, hallColor);
            }
        }

        texture.Apply();
    }

    void DrawRoomOutline(Map.Rect _room)
    {
        if (!_room.isRoom)
            return;

        for(int i=0;i<_room.edgeRect.Count;i++)
        {
            if (_room.edgeRect[i].isRoom)
                continue;
            DrawSideLine(_room, _room.edgeRect[i]);
        }
        for (int i = 0; i < _room.linkedEdgeRect.Count; i++)
        {
            if (_room.linkedEdgeRect[i].isRoom)
                continue;
            DrawSideLine(_room, _room.linkedEdgeRect[i]);
        }
    }

    void DrawSideLine(Map.Rect _roomA, Map.Rect _roomB)
    {
        int start = 0;
        int end = 0;
        int x = 0;
        int y = 0;
        bool leftOrRight = true;
        Direction direction = Check(_roomA, _roomB);
        List<int> yArr = new List<int>(4)
                {
                    _roomB.y * size,
                    (_roomB.y + _roomB.height) * size,
                    _roomA.y * size,
                    (_roomA.y + _roomA.height) * size
                };

        yArr.Sort();
        List<int> xArr = new List<int>(4)
                {
                    _roomB.x * size,
                    (_roomB.x + _roomB.width) * size,
                    _roomA.x * size,
                    (_roomA.x + _roomA.width) * size
                };

        xArr.Sort();
        switch (direction)
        {
            case Direction.LEFT:
                start = yArr[1];
                end = yArr[2];

                x = _roomA.x * size;
                break;
            case Direction.RIGHT:
                start = yArr[1];
                end = yArr[2];

                x = (_roomA.x + _roomA.width) * size;
                break;
            case Direction.TOP:
                start = xArr[1];
                end = xArr[2];

                y = (_roomA.y + _roomA.height) * size;
                leftOrRight = false;
                break;
            case Direction.DOWN:
                start = xArr[1];
                end = xArr[2];

                y = _roomA.y * size;
                leftOrRight = false;
                break;
            default:
                break;
        }

        if (leftOrRight)
        {
            for(int i = start; i< end; i++)
            {
                texture.SetPixel(x, i, Color.black);
            }
        }
        else
        {
            for (int i = start; i < end; i++)
            {
                texture.SetPixel(i, y, Color.black);

            }
        }
    }

    void DrawDoor(Map.Rect _room)
    {
        int minX = _room.x * size;
        int maxX = (_room.x + _room.width) * size - 1;
        int minY = _room.y * size;
        int maxY = (_room.y + _room.height) * size - 1;

        float mapMidX = _room.midX * Map.MapManager.Instance.size;
        float mapMidY = _room.midY * Map.MapManager.Instance.size;


        for (int i = 0; i < _room.doorObjects.Count; i++)
        {
            bool horizon = _room.doorObjects[i].GetComponent<Door>().GetHorizon();

            float gap;

            if (!horizon) // 세로
            {
                gap = mapMidY - _room.doorObjects[i].transform.position.y;

                int pos = (minX + maxX) / 2;

                int doorPos = (int)Mathf.Floor(_room.doorObjects[i].transform.position.x);
                float x = doorPos / mapSizeWidth;
                pos = (int)(x * minmapSizeWidth);
                if (gap < 0)
                {
                    //top
                    texture.SetPixel(pos - 1, maxY + 1, Color.red);
                    texture.SetPixel(pos, maxY + 1, Color.red);
                    texture.SetPixel(pos + 1, maxY + 1, Color.red);
                }
                else
                {
                    //bottom
                    texture.SetPixel(pos - 1, minY, Color.red);
                    texture.SetPixel(pos, minY, Color.red);
                    texture.SetPixel(pos + 1, minY, Color.red);
                }
            }
            else
            {
                gap = mapMidX - _room.doorObjects[i].transform.position.x;

                int pos = (minY + maxY) / 2;

                int doorPos = (int)Mathf.Floor(_room.doorObjects[i].transform.position.y);
                float y = doorPos / mapSizeHeight;
                pos = (int)(y * minmapSizeHeight);

                if (gap < 0)
                {
                    //right
                    texture.SetPixel(maxX + 1, pos - 1, Color.red);
                    texture.SetPixel(maxX + 1, pos, Color.red);
                    texture.SetPixel(maxX + 1, pos + 1, Color.red);
                }
                else
                {
                    //left
                    texture.SetPixel(minX, pos - 1, Color.red);
                    texture.SetPixel(minX, pos, Color.red);
                    texture.SetPixel(minX, pos + 1, Color.red);
                }
            }

        }

        texture.Apply();

    }

    // 0806 윤아 추가
    bool FixedPlayer()
    {
        // 수정해야 하는 부분 : 
        // 두 가지 경우가 모두 들어갈 경우
        bool isX = playerPositon.x >= 34 || playerPositon.x <= 11.1f;
        bool isY = playerPositon.y >= 34 || playerPositon.y <= 11.1f;
        if (isX)
        {
            tanslateMapX = false;
            return true;
        }
        else if (isY)
        {
            tanslateMapX = true;
            return true;
        }
        else if (isX && isY)
        {
            // 수정할 예정
            return true;
        }
        else
        {
            tanslateMapX = false;
            return false;
        }
    }

    void MovePlayerIconMinimap()
    {
        float _width = width / 2f;
        float _height = height / 2f;
        if (tanslateMapX)
        {
            // 맵의 x축은 이동하고 플레이어 아이콘의 y축이 이동
            transform.localPosition = new Vector2(-(playerPositon.x / mapSizeWidth) * width + width / 2 - maskSize,
                mapV.y);
            iconV = new Vector2(-maskSize,
            playerPositon.y / mapSizeHeight * _height - maskSize - _height / 2f);
            playerIcon.transform.localPosition = iconV;
        }
        else
        {
            // 맵의 y축은 이동하고 플레이어 아이콘의 x축이 이동
            transform.localPosition = new Vector2(mapV.x,
            -(playerPositon.y / mapSizeHeight) * height + height / 2 - maskSize - 0.2f);
            iconV = new Vector2(playerPositon.x / mapSizeWidth * _width - maskSize - _width / 2f,
            -maskSize);
            playerIcon.transform.localPosition = iconV;
        }
    }

    void MovePlayerIcon()
    {
        iconV = new Vector2(playerPositon.x / mapSizeWidth * width - maskSize - width / 2,
            playerPositon.y / mapSizeHeight * height - maskSize - height / 2);
        playerIcon.transform.localPosition = iconV;
    } // 현재 플레이어 위치 to MiniMap

    void MoveMinimapIcon()
    {
        mapV = new Vector2(-(playerPositon.x / mapSizeWidth) * width + width / 2 - maskSize,
            -(playerPositon.y / mapSizeHeight) * height + height / 2 - maskSize - 0.2f);
        transform.localPosition = mapV;

        // Icon의 위치를 중앙으로 변경해야함. 한번만 호출해야햐는데 얘를 어디에 둬야 할까?
        playerIcon.transform.localPosition = new Vector2(-maskSize, -maskSize);
    } // 현재 플레이어 위치 to MiniMap

    /** 수정전
    void MovePlayerIcon()
    {
        Vector2 v = new Vector2(playerPositon.x / mapSizeWidth * width - maskSize - width / 2,
            playerPositon.y / mapSizeHeight * height - maskSize - height / 2);
        playerIcon.transform.localPosition = v;
    } // 현재 플레이어 위치 to MiniMap

    void MoveMinimapIcon()
    {
        transform.localPosition = new Vector2(-(playerPositon.x / mapSizeWidth) * width + width / 2 - maskSize,
            -(playerPositon.y / mapSizeHeight) * height + height / 2 - maskSize - 0.2f);
    } // 현재 플레이어 위치 to MiniMap
    **/

    #region UnityFunc
    private void Update()
    {
        playerPositon = PlayerManager.Instance.GetPlayerPosition();

        if (isToggle)
            MovePlayerIcon();
        else
        {
            if (FixedPlayer())
                MovePlayerIconMinimap();
            else
                MoveMinimapIcon();
        }
    }
    #endregion
}
