using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
public class ObjectTile : TileBase
{
    public CustomObject mCustomObject;
    Vector3Int mPosition;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        if (mCustomObject == null)
            return;
        tileData.colliderType = Tile.ColliderType.None;
        mPosition = position;

        tileData.sprite = mCustomObject.sprite;
    }

    public Vector3Int GetPosition()
    {
        return mPosition;
    }
}
