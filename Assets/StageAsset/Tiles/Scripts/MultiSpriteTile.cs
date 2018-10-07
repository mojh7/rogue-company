using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(menuName = "Tile/MultiSpriteTile")]
public class MultiSpriteTile : TileBase
{
    int tileIdx = 0;
    [SerializeField]
    private Sprite[] mSprites;
    public Tile.ColliderType colliderType;
    Vector3Int mPosition;



    public void SetIdx()
    {
        tileIdx = Random.Range(0, mSprites.Length);
    }

    public Sprite GetSprite(Vector3Int position)
    {
        return mSprites[tileIdx];
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        if (mSprites.Length == 0) return;
        Random.InitState(position.GetHashCode());
        tileData.colliderType = colliderType;

        tileData.sprite = mSprites[tileIdx];
    }

    public Vector3Int GetPosition()
    {
        return mPosition;
    }
}
