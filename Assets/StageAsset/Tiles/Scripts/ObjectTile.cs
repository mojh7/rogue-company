using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
public class ObjectTile : TileBase
{
    public Sprite m_Sprite;
    public GameObject m_gameObject;
    Vector3Int m_position;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        if (m_gameObject == null)
            return;
        tileData.colliderType = Tile.ColliderType.None;
        m_position = position;

        tileData.sprite = m_Sprite;
    }

    public Vector3Int GetPosition()
    {
        return m_position;
    }

    public GameObject GetGameObject()
    {
        return m_gameObject;
    }

}
