using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

public class RandomTile : TileBase {
    public SpriteArray[] mSprites;
    public Tile.ColliderType colliderType;
    Vector3Int mPosition;

    public Sprite GetSprite(Vector3Int position)
    {
        Random.InitState(position.GetHashCode());
        int total = mSprites.Sum(x => x.probability);

        float randomPoint = Random.value * total;
        for (int i = 0; i < mSprites.Length; i++)
        {
            if (randomPoint < mSprites[i].probability)
            {
                return mSprites[i].sprite;
            }
            else
            {
                randomPoint -= mSprites[i].probability;
            }
        }

        return mSprites[mSprites.Length - 1].sprite;
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        if (mSprites.Length == 0) return;
        Random.InitState(position.GetHashCode());
        int total = mSprites.Sum(x => x.probability);
        tileData.colliderType = colliderType;
        mPosition = position;

        float randomPoint = Random.value * total;
        for (int i = 0; i < mSprites.Length; i++)
        {
            if (randomPoint < mSprites[i].probability)
            {
                tileData.sprite = mSprites[i].sprite;
                return;
            }
            else
            {
                randomPoint -= mSprites[i].probability;
            }
        }

        tileData.sprite = mSprites[mSprites.Length - 1].sprite;
    }

    public Vector3Int GetPosition()
    {
        return mPosition;
    }

    [System.Serializable]
    public class SpriteArray
    {
        public Sprite sprite;
        [Range(1,100)]
        public int probability = 1;
    }
}
