using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
public class DoorTile : TileBase
{
    enum DoorDir { LEFT, RIGHT, UP, DOWN };

    Vector3Int m_position;
    public Sprite[] m_WallSprite;
    public Sprite[] m_DoorSprite;
    DoorDir doorDir;
    bool isDoor;
    TileData m_tileData;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        if (null == m_WallSprite || null == m_DoorSprite) return;

        tileData.colliderType = Tile.ColliderType.None;
        isDoor = false;
        TileBase leftTile = tilemap.GetTile(new Vector3Int(position.x - 1, position.y, 0));
        TileBase rightTile = tilemap.GetTile(new Vector3Int(position.x + 1, position.y, 0));
        TileBase upTile = tilemap.GetTile(new Vector3Int(position.x, position.y + 1, 0));
        TileBase downTile = tilemap.GetTile(new Vector3Int(position.x, position.y - 1, 0));
        m_position = position;
        if (null == leftTile) // 왼쪽 타일이 null
        {
            doorDir = DoorDir.LEFT;
        }
        else if(null == rightTile){
            doorDir = DoorDir.RIGHT;
        }
        else if(null == upTile)
        {
            doorDir = DoorDir.UP;
        }
        else
        {
            doorDir = DoorDir.DOWN;
        }

        switch (doorDir)
        {
            case DoorDir.LEFT:
                tileData.sprite = m_WallSprite[0];
                break;
            case DoorDir.RIGHT:
                tileData.sprite = m_WallSprite[1];
                break;
            case DoorDir.UP:
                tileData.sprite = m_WallSprite[2];
                break;
            case DoorDir.DOWN:
                tileData.sprite = m_WallSprite[3];
                break;
            default:
                break;
        }
        m_tileData = tileData;
    }

    public void SetDoorable()
    {
        isDoor = true;
        switch (doorDir)
        {
            case DoorDir.LEFT:
                m_tileData.sprite = m_DoorSprite[0];
                break;
            case DoorDir.RIGHT:
                m_tileData.sprite = m_DoorSprite[1];
                break;
            case DoorDir.UP:
                m_tileData.sprite = m_DoorSprite[2];
                break;
            case DoorDir.DOWN:
                m_tileData.sprite = m_DoorSprite[3];
                break;
            default:
                break;
        }
    }

    public Vector3Int GetPosition()
    {
        return m_position;
    }
}
