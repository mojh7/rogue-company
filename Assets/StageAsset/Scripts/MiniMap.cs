using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Threading;


public class MiniMap : MonoBehaviourSingleton<MiniMap>
{
    [SerializeField] private Sprite unknownIcon, monsterIcon, bossIcon, eventIcon, storeIcon;
    [SerializeField] private GameObject playerIcon;
    [SerializeField] private Transform mask;
    [SerializeField] private Text floorT;

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
    void EnableMask()
    {
        mask.GetComponent<Mask>().enabled = !mask.GetComponent<Mask>().enabled;
    }

    void DrawCall(Map.Rect _room,System.Action<Map.Rect> action)
    {
        ThreadStart threadStart = delegate
        {
            action(_room);
        };
        threadStart.Invoke();
    }
    
    public void SetFloorText()
    {
        floorT.text = (5 + GameDataManager.Instance.GetFloor()).ToString() + "F";
    }
    //TODO : 따로 thread로 가능한지 확인
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
        if (!_rect.isClear)
            sprite = unknownIcon;
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
    //TODO : 따로 thread로 가능한지 확인
    public void ClearRoom(Map.Rect _room)
    {
        DrawCall(_room, DrawRoom);
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

        texture.Apply();
    } // 미니맵 그리는 함수

    public void HideMiniMap()
    {
        this.gameObject.SetActive(!this.gameObject.activeSelf);
    }

    void MovePlayerIcon()
    {
        Vector2 v = new Vector2(playerPositon.x / mapSizeWidth * width - maskSize - width / 2,
            playerPositon.y / mapSizeHeight * height - maskSize - height / 2);
        playerIcon.transform.localPosition = v;
    } // 현재 플레이어 위치 to MiniMap

    void MoveMinimapIcon()
    {
        transform.localPosition = new Vector2(-(playerPositon.x / mapSizeWidth) * width + width / 2 - maskSize,
            -(playerPositon.y / mapSizeHeight) * height + height / 2 - maskSize);
    } // 현재 플레이어 위치 to MiniMap

    public void ToggleMinimap()
    {
        EnableMask();
        if (isToggle)
        {
            playerIcon.transform.localPosition = new Vector2(-maskSize, -maskSize);
            mask.localPosition = oldPos;
            GetComponent<RawImage>().color = new Color(1, 1, 1, 1);
        }
        else
        {
            transform.localPosition = new Vector2(-maskSize, -maskSize);
            mask.localPosition = new Vector2(maskSize, maskSize);
            GetComponent<RawImage>().color = new Color(1, 1, 1, 0.3f);
        }
        isToggle = !isToggle;
    }
    #region UnityFunc
    private void Update()
    {
        playerPositon = PlayerManager.Instance.GetPlayerPosition();

        if (isToggle)
            MovePlayerIcon();
        else
            MoveMinimapIcon();
    }
    #endregion
}
