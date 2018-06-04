using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    void EnableMask()
    {
        mask.GetComponent<Mask>().enabled = !mask.GetComponent<Mask>().enabled;
    }

    public void SetFloorText()
    {
        floorT.text = (5 + GameDataManager.Instance.GetFloor()).ToString() + "F";
    }

    public void DrawRoom(Map.Rect _room)
    {
        int gap = size / 2 - 1;
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
                    texture.SetPixel(x, y, Color.white);
                DrawIcon(_room);
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

        mapSizeWidth = Map.MapManager.Instance.size * Map.MapManager.Instance.width; // 실제 맵 크기 48
        mapSizeHeight = Map.MapManager.Instance.size * Map.MapManager.Instance.height;

        if(Map.MapManager.Instance.width * size > Map.MapManager.Instance.height * size)
            GetComponent<RectTransform>().sizeDelta = new Vector2(minimapBaseWidth * (float)mapSizeWidth / mapSizeHeight, minimapBaseHeight);
        else
            GetComponent<RectTransform>().sizeDelta = new Vector2(minimapBaseWidth, minimapBaseHeight * (float)mapSizeHeight / mapSizeWidth);
        oldPos = new Vector2(mask.localPosition.x, mask.localPosition.y);

        width = GetComponent<RectTransform>().sizeDelta.x;
        height = GetComponent<RectTransform>().sizeDelta.y;

        texture = new Texture2D(minmapSizeWidth, minmapSizeHeight);
        texture.filterMode = FilterMode.Point;
        renderer.texture = texture;
        for (int i = 0; i < minmapSizeWidth; i++)
        {
            for (int j = 0; j < minmapSizeHeight; j++)
            {
                texture.SetPixel(i, j, new Color(1,1,1,0.3f));
            }
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            //if(!roomList[i].isRoom)
                DrawRoom(roomList[i]);
        }
        for (int i = 0; i < minmapSizeWidth; i++)
        {
            for (int j = 0; j < minmapSizeHeight; j++)
            {
                if (i == 0 || i == minmapSizeWidth - 1 ||
                    j == 0 || j == minmapSizeHeight - 1)
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

    void MovePlayerIcon()
    {
        Vector2 v = new Vector2(playerPositon.x / mapSizeWidth * width - maskSize - width / 2,
            playerPositon.y / mapSizeHeight * height - maskSize - height / 2 );
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
