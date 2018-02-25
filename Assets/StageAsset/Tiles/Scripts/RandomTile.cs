using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
public class RandomTile : TileBase {
    public SpriteArray[] m_Sprites;
    Vector3Int m_position;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        if (m_Sprites.Length == 0) return;
        Random.InitState(position.GetHashCode());
        int total = m_Sprites.Sum(x => x.probability);
        tileData.colliderType = Tile.ColliderType.None;
        m_position = position;

        float randomPoint = Random.value * total;
        for (int i = 0; i < m_Sprites.Length; i++)
        {
            if (randomPoint < m_Sprites[i].probability)
            {
                tileData.sprite = m_Sprites[i].sprite;
                return;
            }
            else
            {
                randomPoint -= m_Sprites[i].probability;
            }
        }

        tileData.sprite = m_Sprites[m_Sprites.Length - 1].sprite;
    }

    public Vector3Int GetPosition()
    {
        return m_position;
    }

    [System.Serializable]
    public class SpriteArray
    {
        public Sprite sprite;
        [Range(1,100)]
        public int probability = 1;
    }
}
