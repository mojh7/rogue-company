using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour {
    private static MiniMap instance;
    public Sprite unknownIcon, monsterIcon, bossIcon, eventIcon, storeIcon;
    public GameObject playerIcon;

    int minmapSize = 100;
    List<Map.Rect> roomList;
    Texture2D texture;
    new RawImage renderer;
    float width;
    int size;
    int mapSize;

    public static MiniMap GetInstance()
    {
        if(instance == null)
        {
            instance = GameObject.FindObjectOfType<MiniMap>() as MiniMap;
        }
        return instance;    
    }

    public void GetRoomList()
    {
        roomList = RoomManager.Getinstance().GetRoomList();
    } // Mapmanager로부터 방 데이터 로드

    public void DrawRoom(Map.Rect _room)
    {
        for (int x = _room.x * size; x < _room.x * size + _room.width * size; x++)
        {
            for (int y = _room.y * size; y < _room.y * size + _room.height * size; y++)
            {
                if (_room.isRoom &&
                    (x == _room.x * size || x == _room.x * size + _room.width * size - 1 ||
                    y == _room.y * size || y == _room.y * size + _room.height * size - 1))
                {
                    texture.SetPixel(x, y, Color.black);
                }
                else
                {
                    texture.SetPixel(x, y, Color.white);
                }
                DrawIcon(_room);
            }
        }
        texture.Apply();
    }

    public void DrawMinimap()
    {
        texture = new Texture2D(minmapSize, minmapSize);
        texture.filterMode = FilterMode.Point;
        renderer.texture = texture;
        for (int i = 0; i < roomList.Count; i++)
        {
            if(!roomList[i].isRoom)
                DrawRoom(roomList[i]);
        }
        for (int i = 0; i < minmapSize; i++)
        {
            for (int j = 0; j < minmapSize; j++)
            {
                if (i == 0 || i == minmapSize - 1 ||
                    j == 0 || j == minmapSize - 1)
                {
                    texture.SetPixel(i, j, Color.black);
                }
            }
        }
        texture.Apply();
    } // 미니맵 그리는 함수

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
        if(!_rect.isClear)
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
                texture.SetPixel(x + i, y + j, sprite.texture.GetPixel((int)textureRect.x + i, (int)textureRect.y + j));
            }
        }
    } // 방 타입에 따른 미니맵 아이콘 표시

    public void PlayerPositionToMap()
    {
        Vector2 positon = PlayerManager.Getinstance().GetPlayerPosition();
        playerIcon.transform.localPosition = new Vector2(positon.x * width / mapSize - width / 2, positon.y * width / mapSize - width / 2);
    } // 현재 플레이어 위치 to MiniMap

    #region UnityFunc
    private void Awake()
    {
        size = minmapSize / 10; // 미니맵 사이즈
        width = GetComponent<RectTransform>().sizeDelta.x;
        renderer = GetComponent<RawImage>();
        mapSize = Map.MapManager.Getinstance().size * Map.MapManager.Getinstance().width;
    }
    #endregion
}
